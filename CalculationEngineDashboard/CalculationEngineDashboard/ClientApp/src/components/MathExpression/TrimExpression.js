const format = expression => {
    var change = [], result = expression.replace(/ /g, "").replace(/\*\*/g, "^"), _count;
    function replace(index, string){result = result.slice(0, index) + string + result.slice(index + 1)}
    function add(index, string){result = result.slice(0, index) + string + result.slice(index)}
    for (var count = 0; count < result.length; count++){
        if (result[count] == "-"){
            if ("abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890)".includes(result[count - 1])){
                change.push(count);
            }else if (result[count - 1] != "("){
                add(count, "(");
                count++;
                _count = count + 1;
                while ("1234567890.".includes(result[_count])) _count++;
                if (_count < result.length - 1){
                    add(_count, ")");
                }else{
                    add(_count + 2, ")");
                }
            }
        }
    }
    change = change.sort(function(a, b){return a - b});
    const len = change.length;
    for (var count = 0; count < len; count++){replace(change[0] + count * 2, " - "); change.shift()}
    return result.replace(/\*/g, " * ").replace(/\^/g, " ** ").replace(/\//g, " / ").replace(/\+/g, " + ");
}

export const trim = expression => {
            var result = format(expression).replace(/ /g, "").replace(/\*\*/g, "^"), deleting = [];
            const brackets = bracket_pairs(result);
            function bracket_pairs(){
                function findcbracket(str, pos){
                    const rExp = /\(|\)/g;
                    rExp.lastIndex = pos + 1;
                    var depth = 1;
                    while ((pos = rExp.exec(str))) if (!(depth += str[pos.index] == "(" ? 1 : -1 )) {return pos.index}
                }
                function occurences(searchStr, str){
                    var startIndex = 0, index, indices = [];
                    while ((index = str.indexOf(searchStr, startIndex)) > -1){
                        indices.push(index);
                        startIndex = index + 1;
                    }
                    return indices;
                }
                const obrackets = occurences("(", result);
                var cbrackets = [];
                for (var count = 0; count < obrackets.length; count++) cbrackets.push(findcbracket(result, obrackets[count]));
                return obrackets.map((e, i) => [e, cbrackets[i]]);
            }

            function remove(deleting){
                function _remove(index){result = result.slice(0, index) + result.slice(index + 1)}
                const len = deleting.length;
                var deleting = deleting.sort(function(a, b){return a - b});
                for (var count = 0; count < len; count++){
                    _remove(deleting[0] - count);
                    deleting.shift()
                }
            }

            function precedence(operator, position){
                if (!"^/*-+".includes(operator)) return "^/*-+";
                if (position == "l" || position == "w") return {"^": "^", "/": "^", "*": "^/*", "-": "^/*", "+": "^/*-+"}[operator];
                if (position == "r") return {"^": "^", "/": "^/*", "*": "^/*", "-": "^/*-+", "+": "^/*-+"}[operator];
            }

            function strip_bracket(string){
                var result = "", level = 0;
                for (var count = 0; count < string.length; count++){
                    if (string.charAt(count) == "(") level++;
                    if (level == 0) result += string.charAt(count);
                    if (string.charAt(count) == ")") level--;
                }
                return result.replace(/\s{2,}/g, " ");
            }
            for (var count = 0; count < brackets.length; count++){
                const pair = brackets[count];
                if (result[pair[0] - 1] == "(" && result[pair[1] + 1] == ")"){
                    deleting.push(...pair);
                }else{
                    const left = precedence(result[pair[0] - 1], "l"), right = precedence(result[pair[1] + 1], "r");
                    var contents = strip_bracket(result.slice(pair[0] + 1, pair[1])), within = "+";
                    for (var _count = 0; _count < contents.length; _count++) if (precedence(contents[_count], "w").length < precedence(within, "w").length) within = contents[_count];
                    if (/^[0-9]+$/g.test(contents) || contents == ""){
                        deleting.push(...pair);
                        continue;
                    }
                    if (left.includes(within) && right.includes(within)){
                        if (!isNaN(result.slice(pair[0] + 1, pair[1]))){
                            if (Number(result.slice(pair[0] + 1, pair[1])) >= 0 && !"abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890".includes(result[pair[0] - 1])) deleting.push(...pair);
                        }else if (!"abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890".includes(result[pair[0] - 1])) deleting.push(...pair);
                    }
                }
            }
            remove(deleting);
            result = format(result);
            return result;
 }