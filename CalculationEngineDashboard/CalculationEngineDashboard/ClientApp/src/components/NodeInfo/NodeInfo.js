
import { trim } from '../../components/MathExpression/TrimExpression';

function doTrim(matchedNode) {
    const conditions = ['+', '-', '*', '^', '%'];
    const containsOperations = conditions.forEach(el => matchedNode.expressionString.includes(el));
    if (containsOperations) {
        return trim(matchedNode.expressionString);
    }
}
export function Nodeinfo({matchedNode}) {
    if (matchedNode.expressionString != null) {
        matchedNode.expressionString = doTrim(matchedNode);
    }
    return (
        <table>
            <tr>
                <th> Informatie scherm</th>
            </tr>

            <tr>
                <th> Naam</th>
            </tr>
            <tr>
                <td> {matchedNode.name}</td>
            </tr>
            <tr>
                <th> Beschrijving</th>
            </tr>
            <tr>
                <td> {matchedNode.Description}</td>
            </tr>
            <tr>
                <th> uitdrukking</th>
            </tr>
            <tr>
                <td> {matchedNode.expressionString != matchedNode.Value ? matchedNode.expressionString : ""}</td>
            </tr>
            <tr>
                <td> {matchedNode.Value}</td>
            </tr>
            <tr>
                <th> Condities</th>
            </tr>
            <tr>
                <td>
                    {matchedNode.Rules.map((rule, index) => (
                        <div key={index}>Name: {rule.name}
                            <div>rulecondition: {rule.rulecondition.name}</div>
                            <div>&emsp;body: {rule.rulecondition.body}</div>
                            <div>ruleaction: {rule.ruleaction.event}</div>
                            <div>&emsp;action: {rule.ruleaction.body}</div>
                            <hr />
                        </div>
                    ))}
                </td>
            </tr>

            <tr>
                <th> Dependencies</th>
            </tr>
            <tr>
                <td>
                    {matchedNode.Dependencies.map(dependency => (<div>{dependency}</div>))}
                </td>
            </tr>

        </table>
    );
};
