
import React, { Component, useRef, useState, useEffect, useCallback } from 'react';

import { Nodeinfo } from '../NodeInfo/NodeInfo'

import * as ReactDOM from 'react-dom';



export default class ForceDirectedGraph extends Component {
    render() {

        const dat = window.dat;
        const d3 = window.d3;
        const ForceGraph2D = window.ForceGraph2D;

        const useForceUpdate = () => {
            const setToggle = useState(false)[1];
            return () => setToggle(b => !b);
        };

        return (ForceDirectedGraph = ({ nodes,links }) => {
            const fgRef = useRef();
            const [controls] = useState({ 'DAG Orientatie': 'td' });
            const [linkDistanceControl, setlinkDistanceControl] = useState({ 'DaglevelDistance': 80 });
            const forceUpdate = useForceUpdate();
            const [check, setCheck] = useState(false);
            const [ScaleK, setScaleK] = useState(0);
            const [ColorNodes, setColorNodes] = useState(new Set());


            const [cooldownTicks, setCooldownTicks] = useState(undefined);
            const [NodeLevel, setNodeLevel] = useState(0);
            const [highlightNodes, setHighlightNodes] = useState(new Set());
            const [highlightLinks, setHighlightLinks] = useState(new Set());
            const [hoverNode, setHoverNode] = useState(null);
           

            useEffect(() => {
                // add controls GUI
                const gui = new dat.GUI();
                gui.add(controls, 'DAG Orientatie', ['td', 'lr'])
                    .onChange(forceUpdate);
            }, []);

            useEffect(() => {

                if (!check) {
                    let node = nodes.find(element => element.parent === "");
                    if (node != undefined) {
                        const distance = 40;
                        const distRatio = 1 + distance / Math.hypot(node.x, node.y, node.z);
                        fgRef.current.d3Force("collide", d3.forceCollide().radius(d => d.r = getbiggestRadius(nodes) + 2));

                        setScaleK(1);
                    }
                    setCheck(check => !check);
                }
            }, [fgRef]);


            const updateHighlight = () => {
                setHighlightNodes(highlightNodes);
                setHighlightLinks(highlightLinks);
            };

            const handleNodeHover = node => {
                highlightNodes.clear();
                highlightLinks.clear();
                if (node) {
                    highlightNodes.add(node);
                    node.neighbours.forEach(neighbour => highlightNodes.add(neighbour));
                    node.links.forEach(link => highlightLinks.add(link));
                }

                setHoverNode(node || null);
                updateHighlight();
            };

            const handleLinkHover = link => {
                highlightNodes.clear();
                highlightLinks.clear();

                if (link) {
                    highlightLinks.add(link);
                    highlightNodes.add(link.source);
                    highlightNodes.add(link.target);
                }

                updateHighlight();
            };

            const handleClick = useCallback(node => {
                fgRef.current.centerAt(node.x, node.y, 1000);
                fgRef.current.zoom(2 + node.level / 4, 2000);
                if (node.level != NodeLevel) {
                    setNodeLevel(node.level);
                    fgRef.current.d3Force("collide", d3.forceCollide().radius(d => d.r = getbiggestRadius(nodes) + 5));
                }
                fetch("../CalculationEngine/GetNode/?Path=" + node.leaf)
                    .then(res => res.json()).then(data => {
                        ReactDOM.render(<Nodeinfo matchedNode={data} />, document.getElementById('informationBox'));
                    });
            }, [fgRef]);

            function isTooDark(r, g, b) {
                var luma = 0.2126 * r + 0.7152 * g + 0.0722 * b; // per ITU-R BT.709
                return luma < 20;
            }

            function getDarkColor() {
                var color = '#';
                var r = '';
                var g = '';
                var b = '';
                for (var i = 0; i < 2; i++) {
                    r += Math.floor(Math.random() * 10);
                    g += Math.floor(Math.random() * 10);
                    b += Math.floor(Math.random() * 10);
                }
                if (isTooDark(color)) {
                    getDarkColor();
                }
                return color + r + g + b;
            }

            function nodePaint(node, ctx, globalScale) {
                // add rect just for  nodes
                const bckgDimensions = node.__bckgDimensions;

                if (node.color == null && node.module == null) {
                    node.color = getDarkColor();
                    while (ColorNodes.has(node.color)) {
                        node.color = getDarkColor();
                    }
                    ColorNodes.add(node.color);
                }
                else if (node.module != null) {
                    let sourceNode = nodes.find(element => element.leaf === node.module);
                    node.color = sourceNode.color;
                    if (!ColorNodes.has(node.color)) {
                        node.color = getDarkColor();
                        while (ColorNodes.has(node.color)) {
                            node.color = getDarkColor();
                        }
                        ColorNodes.add(node.color);
                    }
                }
                ctx.fillStyle = node.color;
                bckgDimensions && ctx.fillRect(node.x - (bckgDimensions[0]) / 2, node.y - (bckgDimensions[1]) / 2, ...bckgDimensions);
            }

            const paintRing = useCallback((node, ctx, globalScale) => {
                if (highlightNodes.has(node)) {
                    const bckgDimensions = node.__bckgDimensions;
                    const newNumbers = bckgDimensions.map(function increment(number) {
                        return (number) * 1.1;
                    });
                    ctx.fillStyle = node === hoverNode ? 'red' : 'orange';
                    bckgDimensions && ctx.fillRect(node.x - newNumbers[0] / 2, node.y - newNumbers[1] / 2, ...newNumbers);
                    ctx.fill();
                }
            }, [hoverNode]);

            function paintText({ id, x, y, level, color, leaf }, ctx, globalScale) {
                // add rect just for  nodes
                ctx.beginPath();
                const fontSize = 8 - globalScale;
                ctx.fillStyle = ' #FFFFFF';
                ctx.font = '${fontSize}px Sans-Serif';
                ctx.textAlign = 'center';
                ctx.textBaseline = 'middle';
                ctx.fillText(leaf, x, y);  // text
                ctx.closePath();
            }

            function getbiggestRadius(nodes) {
                let biggest = Number.MIN_VALUE;
                nodes.forEach(node => {
                    const bckgDimensions = node.__bckgDimensions;
                    if ((bckgDimensions[0]) / 2 > biggest) {
                        biggest = (bckgDimensions[0] - node.level) / 2;
                    }
                });
                return biggest;
            }
            
            const zoomed = useCallback((coords) => {
                let newDagLevelDistance = 120 - (coords.k * 20);
                if (coords.k < 2) {
                    var x = controls['DAG Orientatie'];
                    if (x == 'td') {
                        newDagLevelDistance = 150 - (coords.k * 30);
                        setlinkDistanceControl({ 'DagLeveldistance': newDagLevelDistance > 80 ? newDagLevelDistance : 80 });
                    }
                    else {
                        newDagLevelDistance = 240 - (coords.k * 30);
                        setlinkDistanceControl({ 'DagLeveldistance': newDagLevelDistance > 200 ? newDagLevelDistance : 200 });
                    }
                }
                else {
                    setlinkDistanceControl({ 'DagLeveldistance': newDagLevelDistance > 40 ? newDagLevelDistance : 40 });
                }
                setScaleK(coords.k);
            }, [fgRef]);


            return <ForceGraph2D
                ref={fgRef}
                graphData={{ nodes, links }}
                width={window.screen.width - (0.3 * window.screen.width)}
                height={window.screen.height - (0.1 * window.screen.height)}
                backgroundColor="#101020"
                dagMode={controls['DAG Orientatie']}
                dagLevelDistance={linkDistanceControl['DagLeveldistance']}
                linkColor={() => 'rgba(255,255,255,0.2)'}
                nodeRelSize={2}
                nodeId="path"
                nodeVal={node => 100 / (node.level + 1)}
                nodeLabel="path"
                nodeAutoColorBy="module"
                linkDirectionalParticleWidth={2}
                d3VelocityDecay={0.5}
                autoPauseRedraw={false}
                linkDirectionalParticles={4}
                linkWidth={link => highlightLinks.has(link) ? 6 : 2}
                linkDirectionalParticleWidth={link => highlightLinks.has(link) ? 6 : 1}
                nodeCanvasObjectMode={(node => 'replace')}
                nodeCanvasObject={(node, ctx, globalScale) => {
                    const label = node.leaf;
                    const fontSize = 12 / globalScale;
                    ctx.font = `${fontSize}px Sans-Serif`;
                    const textWidth = ctx.measureText(label).width;
                    const bckgDimensions = [textWidth, fontSize].map(n => n + fontSize * 4); // some padding
                    node.__bckgDimensions = bckgDimensions;

                    paintRing(node, ctx, globalScale);
                    nodePaint(node, ctx, globalScale);
                    paintText(node, ctx, globalScale);
                }}
                nodePointerAreaPaint={(node, color, ctx, globalScale) => {
                    ctx.fillStyle = color;
                    const bckgDimensions = node.__bckgDimensions;
                    bckgDimensions && ctx.fillRect(globalScale + (node.x - bckgDimensions[0] / 2), (node.y - bckgDimensions[1] / 2), ...bckgDimensions);
                }}
                linkCanvasObjectMode={(link => 'after')}
                linkCanvasObject={(link, ctx, globalScale) => {
                    link.width = link.width
                    link.color = 'rgba(255,255,255,0.2)'

                }}
                onZoom={(k, x, y) => { zoomed(k, x, y) }}
                zoomToFit={(0, 10)
                }
                d3Force={("alphaTarget", 0.03)}
                d3Force={("charge", d3.forceManyBody().strength((d, i) => i ? 0 : -(0.7 * window.screen.width) * 2 / 3))
                }
                enableZoomInteraction={true}
                minZoom={1}
                maxZoom={8}
                d3VelocityDecay={0.3}
                onNodeHover={handleNodeHover}
                onLinkHover={handleLinkHover}
                onEngineStop={() => { setCooldownTicks(0); }}
                onNodeClick={handleClick}
                onDagError={(error) => console.log(error)}
            />;
        });
    }
}