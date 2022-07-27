﻿using GraphX;
using GraphX.Common.Enums;
using GraphX.Common.Interfaces;
using GraphX.Controls;
using GraphX.Logic.Algorithms.LayoutAlgorithms;
using GraphxOrtho.Models;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Media;
using System.Windows.Shapes;
using GraphxOrtho.Models.AlgorithmTools;
using System.Windows.Controls;
using QuickGraph;
using GraphX.Common.Models;

namespace GraphxOrtho
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            ZoomControl.SetViewFinderVisibility(zoomctrl, Visibility.Visible);
            //Set Fill zooming strategy so whole graph will be always visible
            zoomctrl.ZoomToFill();
            
            //Lets setup GraphArea settings
            GraphAreaExample_Setup();
            //OrthoAlgo.Do();

            Loaded += MainWindow_Loaded;
        }
        void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            //lets create graph
            //Note that you can't create it in class constructor as there will be problems with visuals
            gg_but_randomgraph_Click(null, null);
        }


        void gg_but_randomgraph_Click(object sender, RoutedEventArgs e)
        {
            //Lets generate configured graph using pre-created data graph assigned to LogicCore object.
            //Optionaly we set first method param to True (True by default) so this method will automatically generate edges
            //  If you want to increase performance in cases where edges don't need to be drawn at first you can set it to False.
            //  You can also handle edge generation by calling manually Area.GenerateAllEdges() method.
            //Optionaly we set second param to True (True by default) so this method will automaticaly checks and assigns missing unique data ids
            //for edges and vertices in _dataGraph.
            //Note! Area.Graph property will be replaced by supplied _dataGraph object (if any).
            Area.GenerateGraph(true, true);

            /* 
             * After graph generation is finished you can apply some additional settings for newly created visual vertex and edge controls
             * (VertexControl and EdgeControl classes).
             * 
             */

            //This method sets the dash style for edges. It is applied to all edges in Area.EdgesList. You can also set dash property for
            //each edge individually using EdgeControl.DashStyle property.
            //For ex.: Area.EdgesList[0].DashStyle = GraphX.EdgeDashStyle.Dash;
            Area.SetEdgesDashStyle(EdgeDashStyle.Solid);
            Area.SetVerticesDrag(true);
            Area.SetEdgesDrag(true);
            
            //This method sets edges arrows visibility. It is also applied to all edges in Area.EdgesList. You can also set property for
            //each edge individually using property, for ex: Area.EdgesList[0].ShowArrows = true;
            Area.ShowAllEdgesArrows(true);

            //This method sets edges labels visibility. It is also applied to all edges in Area.EdgesList. You can also set property for
            //each edge individually using property, for ex: Area.EdgesList[0].ShowLabel = true;
            Area.ShowAllEdgesLabels(false);

            zoomctrl.ZoomToFill();
        }

        private GraphExample GraphExample_Setup()
        {
            //Lets make new data graph instance
            var dataGraph = new GraphExample();
            //Now we need to create edges and vertices to fill data graph
            //This edges and vertices will represent graph structure and connections
            //Lets make some vertices
            for (int i = 1; i <= 6; i++)
            {
                //Create new vertex with specified Text. Also we will assign custom unique ID.
                //This ID is needed for several features such as serialization and edge routing algorithms.
                //If you don't need any custom IDs and you are using automatic Area.GenerateGraph() method then you can skip ID assignment
                //because specified method automaticaly assigns missing data ids (this behavior is controlled by method param).
                var dataVertex = new DataVertex("V - " + i);
                //Add vertex to data graph
                dataGraph.AddVertex(dataVertex);
            }
            
            //Now lets make some edges that will connect our vertices
            //get the indexed list of graph vertices we have already added
            var vlist = dataGraph.Vertices.ToList(); // Length = 6
            //Then create two edges optionaly defining Text property to show who are connected
            var dataEdge = new DataEdge(vlist[0], vlist[1]) { };
            dataGraph.AddEdge(dataEdge);
            //dataEdge = new DataEdge(vlist[1], vlist[0]) {  };
            //dataGraph.AddEdge(dataEdge);
            dataEdge = new DataEdge(vlist[2], vlist[0]) { };
            dataGraph.AddEdge(dataEdge);
            dataEdge = new DataEdge(vlist[0], vlist[3]) { };
            dataGraph.AddEdge(dataEdge);
            //dataEdge = new DataEdge(vlist[2], vlist[3]) { };
            //dataGraph.AddEdge(dataEdge);
            dataEdge = new DataEdge(vlist[4], vlist[0]) { };
            dataGraph.AddEdge(dataEdge);
            dataEdge = new DataEdge(vlist[5], vlist[4]) { };
            dataGraph.AddEdge(dataEdge);
            dataEdge = new DataEdge(vlist[3], vlist[5]) { };
            dataGraph.AddEdge(dataEdge);
            dataEdge = new DataEdge(vlist[1], vlist[3]) { };
            dataGraph.AddEdge(dataEdge);
            return dataGraph;
        }

        private void GraphAreaExample_Setup()
        {
            //Lets create logic core and filled data graph with edges and vertices
            var logicCore = new GXLogicCoreExample() { Graph = GraphExample_Setup() };
            //This property sets layout algorithm that will be used to calculate vertices positions
            //Different algorithms uses different values and some of them uses edge Weight property.
            logicCore.DefaultLayoutAlgorithm = LayoutAlgorithmTypeEnum.FR;
            //Now we can set parameters for selected algorithm using AlgorithmFactory property. This property provides methods for
            //creating all available algorithms and algo parameters.
            logicCore.DefaultLayoutAlgorithmParams = logicCore.AlgorithmFactory.CreateLayoutParameters(LayoutAlgorithmTypeEnum.KK);
            //Unfortunately to change algo parameters you need to specify params type which is different for every algorithm.
            ((KKLayoutParameters)logicCore.DefaultLayoutAlgorithmParams).MaxIterations = 100;
            
            //This property sets vertex overlap removal algorithm.
            //Such algorithms help to arrange vertices in the layout so no one overlaps each other.
            logicCore.DefaultOverlapRemovalAlgorithm = OverlapRemovalAlgorithmTypeEnum.FSA;
            //Default parameters are created automaticaly when new default algorithm is set and previous params were NULL
            logicCore.DefaultOverlapRemovalAlgorithmParams.HorizontalGap = 50;
            logicCore.DefaultOverlapRemovalAlgorithmParams.VerticalGap = 50;
            logicCore.EnableParallelEdges = true;
            logicCore.ParallelEdgeDistance = 10;

            //This property sets edge routing algorithm that is used to build route paths according to algorithm logic.
            //For ex., SimpleER algorithm will try to set edge paths around vertices so no edge will intersect any vertex.
            //Bundling algorithm will try to tie different edges that follows same direction to a single channel making complex graphs more appealing.
            

            //logicCore.DefaultEdgeRoutingAlgorithm = EdgeRoutingAlgorithmTypeEnum.SimpleER;
            //This property sets async algorithms computation so methods like: Area.RelayoutGraph() and Area.GenerateGraph()
            //will run async with the UI thread. Completion of the specified methods can be catched by corresponding events:
            //Area.RelayoutFinished and Area.GenerateGraphFinished.
            logicCore.AsyncAlgorithmCompute = false;
            //Finally assign logic core to GraphArea object
            logicCore.ExternalEdgeRoutingAlgorithm = new OrthogonalEdgeRoutingAlgorithm<DataVertex, DataEdge>() { Graph = (GraphExample)logicCore.Graph };
            Area.LogicCore = logicCore;
            Area.SetVerticesMathShape(VertexShape.Rectangle);
            System.Console.WriteLine();
        }

        public void Dispose()
        {
            //If you plan dynamicaly create and destroy GraphArea it is wise to use Dispose() method
            //that ensures that all potential memory-holding objects will be released.
            Area.Dispose();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Area.RelayoutGraph(false);
            // Удаляем все линии с графа, чтобы нарисовать новые.
            #region Рисуем оси X и Y и удаляем все предыдущие построения линий и кругов.
            var allLines = Area.GetChildControls<Line>().ToList();
            foreach (var item in allLines)
            {
                Area.Children.Remove(item);
            }
            var allPoints = Area.GetChildControls<Ellipse>().ToList();
            foreach (var item in allPoints)
            {
                Area.Children.Remove(item);
            }

            var yAxis = new Line()
            {
                X1 = 0,
                Y1 = -1000,
                X2 = 0,
                Y2 = 1000,
                Stroke = Brushes.LightGray,
                StrokeThickness = 0.3
            };

            Area.AddCustomChildControl(yAxis);
            var xAxis = new Line()
            {
                X1 = -1000,
                Y1 = 0,
                X2 = 1000,
                Y2 = 0,
                Stroke = Brushes.LightGray,
                StrokeThickness = 0.3
            };
            Area.AddCustomChildControl(xAxis); 
            #endregion

            //var sourcePointOfEdge = GetSourcePointOfEdge(firstEdge);
            var ovgVertices = (Area.LogicCore.ExternalEdgeRoutingAlgorithm as OrthogonalEdgeRoutingAlgorithm<DataVertex, DataEdge>).OvgVertices;
            foreach (var vertex in ovgVertices.Values)
            {
                foreach (var connPoint in vertex.ConnectionPoints.Values)
                {
                    var sourceCircle = new Ellipse()
                    {
                        Width = 4,
                        Height = 4,
                        Stroke = Brushes.Black,
                        StrokeThickness = 1
                    };
                    Area.AddCustomChildControl(sourceCircle);
                    GraphAreaBase.SetX(sourceCircle, connPoint.X - 2);
                    GraphAreaBase.SetY(sourceCircle, connPoint.Y - 2);

                }
            }
            var orthogonalGraph = (Area.LogicCore.ExternalEdgeRoutingAlgorithm as OrthogonalEdgeRoutingAlgorithm<DataVertex, DataEdge>).OrthogonalVisibilityGraph;
            foreach (var edge in orthogonalGraph.AdjacencyGraph.Edges)
            {
                var stroke = Brushes.Green;
                var strokeThickness = 1.0;

                var lineToAdd = new Line()
                {
                    X1 = edge.Source.Point.X,
                    Y1 = edge.Source.Point.Y,
                    X2 = edge.Target.Point.X,
                    Y2 = edge.Target.Point.Y,
                    Stroke = stroke,
                    StrokeThickness = strokeThickness
                };

                Area.AddCustomChildControl(lineToAdd);
                var sourceCircle = new Ellipse()
                {
                    Width = 4,
                    Height = 4,
                    Stroke = Brushes.Black,
                    StrokeThickness = 1
                };
                Area.AddCustomChildControl(sourceCircle);
                GraphAreaBase.SetX(sourceCircle, edge.Source.Point.X - 2);
                GraphAreaBase.SetY(sourceCircle, edge.Source.Point.Y - 2);
            }
            return;
            //var vertexOfFirstEdge = firstEdge.Value.Source;

            List<OrthogonalVertex> orthogonalVertices = new List<OrthogonalVertex>();

            #region Ищем крайнюю девую и правую точку.

            var zoomctrl = Area.Parent as ZoomControl;
            Point leftTopEndOfGraph = new Point(0, 0);
            Point rightBottomEndOfGraph = new Point(0, 0);
            foreach (var vertex in Area.VertexList)
            {
                // словарь узлов
                // локальный объект - DataVertex
                var dataKey = vertex.Key;
                // визуальный объект - VertexControl
                var dataControl = vertex.Value;
                Point positionOfNode = dataControl.GetPosition();
                if (positionOfNode.X + dataControl.ActualWidth > rightBottomEndOfGraph.X)
                    rightBottomEndOfGraph.X = positionOfNode.X + dataControl.ActualWidth;
                if (positionOfNode.X < leftTopEndOfGraph.X)
                    leftTopEndOfGraph.X = positionOfNode.X;
                if (positionOfNode.Y + dataControl.ActualHeight > rightBottomEndOfGraph.Y)
                    rightBottomEndOfGraph.Y = positionOfNode.Y + dataControl.ActualHeight;
                if (positionOfNode.Y < leftTopEndOfGraph.Y)
                    leftTopEndOfGraph.Y = positionOfNode.Y;
                // creating vertex with bounds and vertical + horizontal segments
            } 
            #endregion
            
            foreach (var vertex in Area.VertexList)
            {
                // визуальный объект - VertexControl
                var dataControl = vertex.Value;
                var rootArea = dataControl.RootArea;
                var relatedEdges = rootArea.GetRelatedVertexControls(dataControl);
                // creating vertex with bounds and vertical + horizontal segments
                orthogonalVertices.Add(new OrthogonalVertex(dataControl, leftTopEndOfGraph, rightBottomEndOfGraph, 10.0));
                //AddBoundSegmentsToAreaByVertexControl(dataControl, zoomctrl);
                var sourceCircle = new Ellipse()
                {
                    Width = 4,
                    Height = 4,
                    Stroke = Brushes.Red,
                    StrokeThickness = 1
                };
                Area.AddCustomChildControl(sourceCircle);
                GraphAreaBase.SetX(sourceCircle, dataControl.GetPosition().X - 2);
                GraphAreaBase.SetY(sourceCircle, dataControl.GetPosition().Y - 2);
            }
            
            

            OrthogonalVisibilityGraph orthogonalVisibilityGraph = new OrthogonalVisibilityGraph(orthogonalVertices);

            var adjGraphEdges = orthogonalVisibilityGraph.AdjacencyGraph.Edges.ToList();
            adjGraphEdges = adjGraphEdges.OrderBy(x => x.Source.Point.X).ToList();

            for (int i = 0; i < adjGraphEdges.Count; i++)
            {
                var stroke = Brushes.Green;
                var strokeThickness = 1.0;

                var lineToAdd = new Line()
                {
                    X1 = adjGraphEdges[i].Source.Point.X,
                    Y1 = adjGraphEdges[i].Source.Point.Y,
                    X2 = adjGraphEdges[i].Target.Point.X,
                    Y2 = adjGraphEdges[i].Target.Point.Y,
                    Stroke = stroke,
                    StrokeThickness = strokeThickness
                };
                //if (IsLineHorizontal(lineToAdd))
                //{
                //    lineToAdd.X1 = lineToAdd.X1;
                //    lineToAdd.X2 = lineToAdd.X2;
                //}
                //else
                //{
                //    lineToAdd.Y1 = lineToAdd.Y1;
                //    lineToAdd.Y2 = lineToAdd.Y2;
                //}
                Area.AddCustomChildControl(lineToAdd);
                var sourceCircle = new Ellipse()
                {
                    Width = 4,
                    Height = 4,
                    Stroke = Brushes.Black,
                    StrokeThickness = 1
                };
                Area.AddCustomChildControl(sourceCircle);
                GraphAreaBase.SetX(sourceCircle, adjGraphEdges[i].Source.Point.X - 2);
                GraphAreaBase.SetY(sourceCircle, adjGraphEdges[i].Source.Point.Y - 2);
            }
            foreach (var vertex in orthogonalVisibilityGraph.AdjacencyGraph.Vertices)
            {
                IEnumerable<Edge<PointWithDirection>> edges;
                orthogonalVisibilityGraph.AdjacencyGraph.TryGetOutEdges(vertex, out edges);
                System.Console.WriteLine();
            }
        }
        
        private static bool IsLineHorizontal(Line line)
        {
            return line.Y1 == line.Y2;
        }
        
    }
    
}
