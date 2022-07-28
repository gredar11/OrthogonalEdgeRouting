using GraphX;
using GraphX.Common.Enums;
using GraphX.Controls;
using GraphX.Logic.Algorithms.LayoutAlgorithms;
using GraphX.Logic.Algorithms.EdgeRouting;
using GraphxOrtho.Models;
using System.Linq;
using System.Windows;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Collections.Generic;
using GraphxOrtho.Models.AlgorithmTools;
using System.Collections;
using GraphxOrtho.Models.OrthogonalTools;

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
            for (int i = 1; i <= 2; i++)
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
            //dataEdge = new DataEdge(vlist[2], vlist[0]) { };
            //dataGraph.AddEdge(dataEdge);
            //dataEdge = new DataEdge(vlist[0], vlist[3]) { };
            //dataGraph.AddEdge(dataEdge);
            ////dataEdge = new DataEdge(vlist[2], vlist[3]) { };
            ////dataGraph.AddEdge(dataEdge);
            //dataEdge = new DataEdge(vlist[4], vlist[0]) { };
            //dataGraph.AddEdge(dataEdge);
            //dataEdge = new DataEdge(vlist[5], vlist[4]) { };
            //dataGraph.AddEdge(dataEdge);
            //dataEdge = new DataEdge(vlist[3], vlist[5]) { };
            //dataGraph.AddEdge(dataEdge);
            //dataEdge = new DataEdge(vlist[1], vlist[3]) { };
            //dataGraph.AddEdge(dataEdge);
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

                if(edge.Source.Point.X == edge.Target.Point.X)
                {
                    var lineToAdd = new Line()
                    {
                        X1 = edge.Source.Point.X,
                        Y1 = edge.Source.Point.Y + (edge.Source.Point.Y > edge.Target.Point.Y ? -1 : 1),
                        X2 = edge.Target.Point.X,
                        Y2 = edge.Target.Point.Y + (edge.Source.Point.Y > edge.Target.Point.Y ? 1 : -1),
                        Stroke = stroke,
                        StrokeThickness = strokeThickness
                    };

                    Area.AddCustomChildControl(lineToAdd);
                }
                if(edge.Source.Point.Y == edge.Target.Point.Y)
                {
                    var lineToAdd = new Line()
                    {
                        X1 = edge.Source.Point.X + (edge.Source.Point.X > edge.Target.Point.X ? -1 : 1),
                        Y1 = edge.Source.Point.Y,
                        X2 = edge.Target.Point.X - (edge.Source.Point.X > edge.Target.Point.X ? -1 : 1),
                        Y2 = edge.Target.Point.Y,
                        Stroke = stroke,
                        StrokeThickness = strokeThickness
                    };

                    Area.AddCustomChildControl(lineToAdd);
                }
                
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
            var algorithmBaseClass = (Area.LogicCore.ExternalEdgeRoutingAlgorithm as OrthogonalEdgeRoutingAlgorithm<DataVertex, DataEdge>);
            var edgeToDraw = algorithmBaseClass.Graph.Edges.First();
            var startVertex = algorithmBaseClass.OvgVertices[edgeToDraw.Source];
            var endVertex = algorithmBaseClass.OvgVertices[edgeToDraw.Target];
            var startPoint = startVertex.ConnectionPoints[edgeToDraw];
            var endPoint = endVertex.ConnectionPoints[edgeToDraw];
            var orthogonalVertices = algorithmBaseClass.OrthogonalVisibilityGraph.AdjacencyGraph.Vertices;

            var strartPointInAdjacecnyGraph = (from v in orthogonalVertices where v.Point == startPoint select v).FirstOrDefault();
            strartPointInAdjacecnyGraph.Direction = startVertex.GetDirectionOfPoint(strartPointInAdjacecnyGraph.Point);
            var endPointInAdjacecnyGraph = (from v in orthogonalVertices where v.Point == endPoint select v).FirstOrDefault();
            endPointInAdjacecnyGraph.Direction = endVertex.GetDirectionOfPoint(endPointInAdjacecnyGraph.Point);

            PriorityPoint start = new PriorityPoint(strartPointInAdjacecnyGraph, null);
            PriorityPoint end = new PriorityPoint(endPointInAdjacecnyGraph, null);
            PriorityAlgorithm<DataVertex,DataEdge> algorithm = new PriorityAlgorithm<DataVertex, DataEdge>(start, end, algorithmBaseClass.OrthogonalVisibilityGraph);
            //var path = algorithm.CalculatePath();
            return;
        }
        public class PointWdComparer : IComparer
        {
            public int Compare(object x, object y)
            {
                throw new System.NotImplementedException();
            }
        }
        private static bool IsLineHorizontal(Line line)
        {
            return line.Y1 == line.Y2;
        }
        
    }
    
}
