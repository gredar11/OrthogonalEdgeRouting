using GraphX.Common.Enums;
using GraphX.Controls;
using GraphX.Logic.Algorithms.LayoutAlgorithms;
using GraphxOrtho.Models;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using GraphX.Measure;
using System.Windows.Shapes;
using System.Windows.Media;
using System.Windows.Data;
using System.Windows.Controls;
using GraphX.Logic.Algorithms.EdgeRouting;
using GraphX.Common.Interfaces;
using GraphX;

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
            Area.ShowAllEdgesLabels(true);

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
            dataEdge = new DataEdge(vlist[1], vlist[0]) {  };
            dataGraph.AddEdge(dataEdge);
            dataEdge = new DataEdge(vlist[2], vlist[0]) { };
            dataGraph.AddEdge(dataEdge);
            dataEdge = new DataEdge(vlist[0], vlist[3]) { };
            dataGraph.AddEdge(dataEdge);
            dataEdge = new DataEdge(vlist[2], vlist[3]) { };
            dataGraph.AddEdge(dataEdge);
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
            logicCore.DefaultLayoutAlgorithm = LayoutAlgorithmTypeEnum.KK;
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
            //logicCore.ExternalEdgeRoutingAlgorithm = new OrthogonalEdgeRoutingAlgorithm<DataVertex,DataEdge>();
            logicCore.DefaultEdgeRoutingAlgorithm = EdgeRoutingAlgorithmTypeEnum.PathFinder;
            //This property sets async algorithms computation so methods like: Area.RelayoutGraph() and Area.GenerateGraph()
            //will run async with the UI thread. Completion of the specified methods can be catched by corresponding events:
            //Area.RelayoutFinished and Area.GenerateGraphFinished.
            logicCore.AsyncAlgorithmCompute = false;
            //Finally assign logic core to GraphArea object
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
            foreach (var vertex in Area.VertexList)
            {
                // словарь узлов
                // локальный объект - DataVertex
                var dataKey = vertex.Key;
                // визуальный объект - VertexControl
                var dataControl = vertex.Value;

                dataControl.SetConnectionPointsVisibility(true);
                var vertexConnectionPoints = dataControl.VertexConnectionPointsList.ToList();

                dataKey.Text = dataControl.GetPosition().X.ToString("F1") + " : " + dataControl.GetPosition().Y.ToString("F1");

                // установка этикетке узла его позиции
                //var vertexTemplate = dataControl.Template;
                //var temp = vertexTemplate.FindName("PART_vertexLabel",dataControl) as VertexLabelControl;
                //temp.Content = dataKey.Text;

            }
            foreach(var edge in Area.EdgesList)
            {
                var edgeKey = edge.Key;
                var edgeData = edge.Value;
                var commonEdge = edgeKey as IGraphXCommonEdge;
                System.Windows.Point sourceConnPoint = new System.Windows.Point();
                var routedEdge = edgeData.Edge as IRoutingInfo;
                var routeInformation = routedEdge.RoutingPoints;
                var hasRouteInfo = routeInformation != null && routeInformation.Length > 1;

                var sourceSize = new System.Windows.Size
                {
                    Width = edgeData.Source.ActualWidth,
                    Height = edgeData.Source.ActualHeight
                };
                var sourcePos = new System.Windows.Point
                {
                    X = (GraphAreaBase.GetFinalX(edgeData.Source)) + sourceSize.Width * 0.5,
                    Y = (GraphAreaBase.GetFinalY(edgeData.Source)) + sourceSize.Height * 0.5
                };
                var sourcePos1 = new System.Windows.Point
                {
                    X = (GraphAreaBase.GetFinalX(edgeData.Source)),
                    Y = (GraphAreaBase.GetFinalY(edgeData.Source))
                };
                var targetPos = new System.Windows.Point
                {
                    X = (GraphAreaBase.GetFinalX(edgeData.Target)),
                    Y = (GraphAreaBase.GetFinalY(edgeData.Target))
                };
                if (commonEdge?.SourceConnectionPointId != null)
                {
                    var sourceCp = edgeData.Source.GetConnectionPointById(commonEdge.SourceConnectionPointId.Value, true);
                    if (sourceCp == null)
                    {
                        throw new System.Exception("");
                    }
                    
                }
                else
                    sourceConnPoint = GeometryHelper.GetEdgeEndpoint(sourcePos, new System.Windows.Rect(sourcePos1, sourceSize), (hasRouteInfo ? routeInformation[1].ToWindows() : (targetPos)), edgeData.Source.VertexShape);
                Area.AddCustomChildControl(new Line()
                {
                    Stroke = Brushes.Red,
                    X1 = sourceConnPoint.X,
                    X2 = sourceConnPoint.X,
                    Y1 = sourceConnPoint.Y - 5,
                    Y2 = sourceConnPoint.Y + 5,
                    StrokeThickness = 2
                });
                Area.AddCustomChildControl(new Line()
                {
                    Stroke = Brushes.Red,
                    X1 = sourceConnPoint.X-5,
                    X2 = sourceConnPoint.X+5,
                    Y1 = sourceConnPoint.Y,
                    Y2 = sourceConnPoint.Y,
                    StrokeThickness = 2
                });
            }
            //var edges = Area.EdgesList.Values.ToList();
            //var vertex1 = edges[0].Source;
            //var currentPosition = vertex1.GetPosition();
            //vertex1.SetPosition(new System.Windows.Point(currentPosition.X - 20, currentPosition.Y - 10));
            //Area.AddCustomChildControl(new Line()
            //{
            //    Stroke = Brushes.Black,
            //    X1 = currentPosition.X,
            //    X2 = currentPosition.X + 100,
            //    Y1 = currentPosition.Y,
            //    Y2 = currentPosition.Y + 100,
            //    StrokeThickness = 1
            //});

            

        }
    }
    
}
