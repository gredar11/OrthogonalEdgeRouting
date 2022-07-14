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
            logicCore.DefaultEdgeRoutingAlgorithm = EdgeRoutingAlgorithmTypeEnum.SimpleER;
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
            // Удаляем все линии с графа, чтобы нарисовать новые.
            var allLines = Area.GetChildControls<Line>().ToList();
            foreach (var item in allLines)
            {
                Area.Children.Remove(item);
            }

            var zoomctrl = Area.Parent as ZoomControl;

            List<OrthogonalVertex> orthogonalVertices = new List<OrthogonalVertex>();
            foreach (var vertex in Area.VertexList)
            {
                // словарь узлов
                // локальный объект - DataVertex
                var dataKey = vertex.Key;
                // визуальный объект - VertexControl
                var dataControl = vertex.Value;
                orthogonalVertices.Add(new OrthogonalVertex(dataControl, zoomctrl.ActualHeight, zoomctrl.ActualWidth, 5.0));
                //AddBoundSegmentsToAreaByVertexControl(dataControl, zoomctrl);

                #region Добавление узлу его текущей позиции
                //dataControl.SetConnectionPointsVisibility(true);
                //var vertexConnectionPoints = dataControl.VertexConnectionPointsList.ToList();

                //dataKey.Text = dataControl.GetPosition().X.ToString("F1") + " : " + dataControl.GetPosition().Y.ToString("F1");

                // установка этикетке узла его позиции
                //var vertexTemplate = dataControl.Template;
                //var temp = vertexTemplate.FindName("PART_vertexLabel",dataControl) as VertexLabelControl;
                //temp.Content = dataKey.Text; 
                #endregion
            }
            OrthogonalVisibilityGraph graph = new OrthogonalVisibilityGraph(orthogonalVertices);
            List<Line> horizontalSegments = new List<Line>();
            List<Line> verticalSegments = new List<Line>();
            
            #region Границы графа
            var horBounder1 = new Line()
            {
                Name = "Line2",
                Stroke = Brushes.Gray,
                X1 = 0,
                X2 = zoomctrl.ActualWidth,
                Y1 = 0,
                Y2 = 0,
                StrokeThickness = 0.5
            };
            var verBounder1 = (new Line()
            {
                Name = "Line2",
                Stroke = Brushes.Gray,
                X1 = 0,
                X2 = 0,
                Y1 = 0,
                Y2 = zoomctrl.ActualHeight,
                StrokeThickness = 0.5
            });
            var horBounder2 = (new Line()
            {
                Name = "Line2",
                Stroke = Brushes.Gray,
                X1 = 0,
                X2 = zoomctrl.ActualWidth,
                Y1 = zoomctrl.ActualHeight,
                Y2 = zoomctrl.ActualHeight,
                StrokeThickness = 0.5
            });
            var verBounder2 = (new Line()
            {
                Name = "Line2",
                Stroke = Brushes.Gray,
                X1 = zoomctrl.ActualWidth,
                X2 = zoomctrl.ActualWidth,
                Y1 = 0,
                Y2 = zoomctrl.ActualHeight,
                StrokeThickness = 0.5
            });
            #endregion
            horizontalSegments.Add(horBounder1);
            horizontalSegments.Add(horBounder2);
            verticalSegments.Add(verBounder1);
            verticalSegments.Add(verBounder2);
            foreach (var orthogonalVertex in orthogonalVertices)
            {
                foreach(var segment in orthogonalVertex.HorizontalSegments)
                {
                    Area.AddCustomChildControl(segment);
                    horizontalSegments.Add(segment);
                }
                foreach (var segment in orthogonalVertex.VerticalSegments)
                {
                    Area.AddCustomChildControl(segment);
                    verticalSegments.Add(segment);
                }
            }
            List<PointWithDirection> pointsForOvg = new List<PointWithDirection>();
            Dictionary<Line, List<PointWithDirection>> segmentsWithPoints = new Dictionary<Line, List<PointWithDirection>>();
            foreach (var hsegment in horizontalSegments)
            {
                foreach (var vsegment in verticalSegments)
                {
                    var intersection = OrthogonalVisibilityGraph.GetIntersectionOfTwoLines(hsegment, vsegment);
                    if (intersection != null && !pointsForOvg.Contains(intersection))
                    {
                        pointsForOvg.Add(intersection);
                        if(!segmentsWithPoints.ContainsKey(hsegment) ||  segmentsWithPoints[hsegment] == null)
                        {
                            segmentsWithPoints[hsegment] = new List<PointWithDirection>();
                        }
                        segmentsWithPoints[hsegment].Add(intersection);
                        if (!segmentsWithPoints.ContainsKey(vsegment) || segmentsWithPoints[vsegment] == null)
                        {
                            segmentsWithPoints[vsegment] = new List<PointWithDirection>();
                        }
                        segmentsWithPoints[vsegment].Add(intersection);
                    }
                }
            }
            foreach (var lineSegment in segmentsWithPoints)
            {
                if (IsLineHorizontal(lineSegment.Key))
                {
                    var points = lineSegment.Value;
                    var pointssortedByX = (points.OrderBy(l => l.Point.X)).ToList();
                    if (pointssortedByX.Count() <= 1)
                        continue;
                    for (int i = 1; i < pointssortedByX.Count; i++)
                    {
                        Area.AddCustomChildControl(new Line()
                        {
                            X1 = pointssortedByX[i - 1].Point.X,
                            Y1 = pointssortedByX[i - 1].Point.Y,
                            X2 = pointssortedByX[i].Point.X,
                            Y2 = pointssortedByX[i].Point.Y,
                            StrokeThickness = 0.5,
                            Stroke = Brushes.Lime,
                            StrokeDashArray = new DoubleCollection() { 1.0 }
                        });
                    }
                }
                else
                {
                    var points = lineSegment.Value;
                    var pointssortedByY = (points.OrderBy(l => l.Point.Y)).ToList();
                    if (pointssortedByY.Count() <= 1)
                        continue;
                    for (int i = 1; i < pointssortedByY.Count; i++)
                    {
                        Area.AddCustomChildControl(new Line()
                        {
                            X1 = pointssortedByY[i - 1].Point.X,
                            Y1 = pointssortedByY[i - 1].Point.Y,
                            X2 = pointssortedByY[i].Point.X,
                            Y2 = pointssortedByY[i].Point.Y,
                            StrokeThickness = 0.5,
                            Stroke = Brushes.Lime,
                            StrokeDashArray = new DoubleCollection() { 1.0 }
                        });
                    }

                }
            }
            foreach(var point in pointsForOvg)
            {
                var circle = new Ellipse()
                {
                    Width = 4,
                    Height = 4,
                    Stroke = Brushes.Black,
                    StrokeThickness = 1
                };
                Area.AddCustomChildControl(circle);
                circle.Margin = new Thickness() { Left = point.Point.X - circle.Width/2, Top = point.Point.Y - circle.Height/2 };
            }
            
            //foreach (var edge in Area.EdgesList)
            //{
            //    //System.Windows.Point sourceConnPoint = GetSourcePointOfEdge(edge); 
            //    var lines = Area.GetChildControls<Line>();
            //}

        }
        public void AddBoundSegmentsToAreaByVertexControl(VertexControl vertex, ZoomControl zoomControl)
        {
            var position = vertex.GetPosition();

            var zoomctrl = Area.Parent as ZoomControl;

            // drawing horizontal segments
            Area.AddCustomChildControl(new Line()
            {
                Stroke = Brushes.Gray,
                X1 = 0,
                X2 = zoomctrl.ActualWidth,
                Y1 = position.Y,
                Y2 = position.Y,
                StrokeThickness = 0.5
            });
            Area.AddCustomChildControl(new Line()
            {
                Stroke = Brushes.Gray,
                X1 = 0,
                X2 = zoomctrl.ActualWidth,
                Y1 = position.Y + vertex.ActualHeight,
                Y2 = position.Y + vertex.ActualHeight,
                StrokeThickness = 0.5
            });
            // drawing vertical segments
            Area.AddCustomChildControl(new Line()
            {
                Stroke = Brushes.Gray,
                X1 = position.X,
                X2 = position.X,
                Y1 = 0,
                Y2 = zoomControl.ActualHeight,
                StrokeThickness = 0.5
            });
            Area.AddCustomChildControl(new Line()
            {
                Stroke = Brushes.Gray,
                X1 = position.X + vertex.ActualWidth,
                X2 = position.X + vertex.ActualWidth,
                Y1 = 0,
                Y2 = zoomControl.ActualHeight,
                StrokeThickness = 0.5
            });
        }
        private static System.Windows.Point GetSourcePointOfEdge(KeyValuePair<DataEdge, EdgeControl> edge)
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
            return sourceConnPoint;
        }
        
        private bool IsLineHorizontal(Line line)
        {
            return line.Y1 == line.Y2;
        }
    }
    
}
