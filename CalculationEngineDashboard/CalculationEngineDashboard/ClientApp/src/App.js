import React, { Component } from 'react';
import ForceDirectedGraph from './components/ForceDirectedGraph/ForceDirectedGraph';

export default class App extends Component {

    constructor(props) {
        super(props);
        this.state = {
            error: null,
            isLoaded: false,
            nodes: [],
            links: []
        };
    }


    componentDidMount() {
        this.populateNodesandLinks();
    }

    render() {
        const { error, isLoaded} = this.state;
            if (error) {
                return <div>Error: {error.message}</div>;
            } else if (!isLoaded) {
            return <div>Loading...</div>;
        } else {
            return (
                <ul>
                    return (
                    <ForceDirectedGraph nodes={this.state.nodes} links={this.state.links} />,
                    document.getElementById('graph'));
                </ul>
            );
        }
    }

    async populateNodesandLinks() {
        console.log("werkt");
        const nodes = [], links = [];
        const response = await fetch('calculationengine');
        await response.json()
            .then(data => {
                console.log(data);
                data.forEach((path) => {
                    const levels = path.split('/'),
                        level = levels.length,
                        module = level > 0 ? levels[1] : null,
                        leaf = levels.pop(),
                        parent = levels.join('/');
                    const node = {
                        path,
                        leaf,
                        module,
                        size: 20,
                        level,
                        neighbours: [],
                        links: [],
                        parent
                    };

                    let sourceNode = nodes.find(element => element.leaf === leaf);
                    if (sourceNode !== undefined) {
                        if (sourceNode.parent != null && sourceNode.parent.length > 0) {
                            links.push({ source: parent, target: sourceNode.parent + '/' + sourceNode.leaf, targetNode: sourceNode });
                        }
                    }
                    else {
                        if (node.parent !== null && node.parent.length > 0) {
                            const levels2 = parent.split('/'),
                                leaf2 = levels2.pop();
                            let sourceNode2 = nodes.find(element => element.leaf === leaf2);
                            if (sourceNode2 !== undefined && parent !== sourceNode2.leaf) {
                                node.path = sourceNode2.path + '/' + leaf;
                                node.module = sourceNode2.module;
                                nodes.push(node);
                                links.push({ source: sourceNode2.path, target: node.path, targetNode: sourceNode2 });
                            }
                            else {
                                links.push({ source: parent, target: path, targetNode: node });
                                nodes.push(node);
                            }

                        }
                        else {
                            nodes.push(node);
                        }
                    }
                });
                links.forEach(link => {
                    const levels = link.source.split('/'),
                        leaf = levels.pop(),
                        levels2 = link.target.split('/'),
                        leaf2 = levels2.pop();
                    const a = nodes.find(element => element.leaf === leaf);
                    const b = nodes.find(element => element.leaf === leaf2);
                    !a.neighbours && (a.neighbours = []);
                    !b.neighbours && (b.neighbours = []);
                    a.neighbours.push(b);
                    b.neighbours.push(a);

                    !a.links && (a.links = []);
                    !b.links && (b.links = []);
                    a.links.push(link);
                    b.links.push(link);
                });
                console.log(nodes, links);
                this.state.nodes = nodes;
                this.state.links = links;
            })
            .then(
                () => {
                    this.setState({
                        isLoaded: true,
                        nodes: nodes,
                        links: links
                    });
                },
                // Note: it's important to handle errors here
                // instead of a catch() block so that we don't swallow
                // exceptions from actual bugs in components.
                (error) => {
                    this.setState({
                        isLoaded: true,
                        error
                    });
                }
            );
        
    }
}
