﻿using GraphX;
using GraphX.Common.Enums;
using GraphX.Controls;
using GraphX.Logic.Algorithms.LayoutAlgorithms;
using GraphxOrtho.Models;
using System.Linq;
using System.Windows;
using System.Windows.Media;
using System.Windows.Shapes;
using Oer = GraphXOrthogonalEr.AlgorithmTools;
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
            //lets create graph //Note that you can't create it in class constructor as there will be problems with visuals
            gg_but_randomgraph_Click(null, null);
        }


        void gg_but_randomgraph_Click(object sender, RoutedEventArgs e)
        {
            
            Area.GenerateGraph(true, true);
            
            Area.SetEdgesDashStyle(EdgeDashStyle.Solid);
            Area.SetVerticesDrag(true);
            Area.SetEdgesDrag(true);
            Area.ShowAllEdgesArrows(true);
            Area.ShowAllEdgesLabels(false);

            zoomctrl.ZoomToFill();
        }

        private GraphExample GraphExample_Setup()
        {
            //Lets make new data graph instance
            var dataGraph = new GraphExample();
            int countOfNodes = 20;
            int countOfEdges = 35;
            for (int i = 1; i <= countOfNodes; i++)
            {
                
                var dataVertex = new DataVertex("V - " + i);
                //Add vertex to data graph
                dataGraph.AddVertex(dataVertex);
            }
            
            var vlist = dataGraph.Vertices.ToList(); // Length = 6
            //Then create two edges optionaly defining Text property to show who are connected
            var rand = new System.Random(1);
            for (int i = 0; i < countOfEdges; i++)
            {
                int startV = GiveMeANumber(-1, countOfNodes, rand);
                int endV = GiveMeANumber(startV, countOfNodes, rand);
                var dataEdge = new DataEdge(vlist[startV], vlist[endV]) { };
                dataGraph.AddEdge(dataEdge);
            }

            return dataGraph;
        }
        private int GiveMeANumber(int excludeInt, int rangeSize, System.Random random)
        {
            var range = Enumerable.Range(0, rangeSize - 1).Where(i => i != excludeInt);
            int index = random.Next(0, rangeSize - 2);
            return range.ElementAt(index);
        }
        private void GraphAreaExample_Setup()
        {
            //Lets create logic core and filled data graph with edges and vertices
            var logicCore = new GXLogicCoreExample() { Graph = GraphExample_Setup() };
            //This property sets layout algorithm that will be used to calculate vertices positions
            //Different algorithms uses different values and some of them uses edge Weight property.
            logicCore.DefaultLayoutAlgorithm = LayoutAlgorithmTypeEnum.ISOM;
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

            logicCore.AsyncAlgorithmCompute = false;
            //Finally assign logic core to GraphArea object
            //logicCore.ExternalEdgeRoutingAlgorithm = new OrthogonalEdgeRoutingAlgorithm<DataVertex, DataEdge>() { Graph = (GraphExample)logicCore.Graph };
            logicCore.ExternalEdgeRoutingAlgorithm = new Oer.OrthogonalEdgeRoutingAlgorithm<DataVertex, DataEdge>() { Graph = (GraphExample)logicCore.Graph };

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
            Area.RelayoutGraph();
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
            var ovgVertices = (Area.LogicCore.ExternalEdgeRoutingAlgorithm as Oer.OrthogonalEdgeRoutingAlgorithm<DataVertex, DataEdge>).OvgVertices;
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
            var orthogonalGraph = (Area.LogicCore.ExternalEdgeRoutingAlgorithm as Oer.OrthogonalEdgeRoutingAlgorithm<DataVertex, DataEdge>).OrthogonalVisibilityGraph;
            foreach (var edge in orthogonalGraph.BiderectionalGraph.Edges)
            {
                var stroke = Brushes.LightGray;
                var strokeThickness = 0.5;

                if (edge.Source.Point.X == edge.Target.Point.X)
                {
                    var lineToAdd = new Line()
                    {
                        X1 = edge.Source.Point.X,
                        Y1 = edge.Source.Point.Y + (edge.Source.Point.Y > edge.Target.Point.Y ? -0.2 : 0.2),
                        X2 = edge.Target.Point.X,
                        Y2 = edge.Target.Point.Y + (edge.Source.Point.Y > edge.Target.Point.Y ? 0.2 : -0.2),
                        Stroke = stroke,
                        StrokeThickness = strokeThickness
                    };

                    Area.AddCustomChildControl(lineToAdd);
                }
                if (edge.Source.Point.Y == edge.Target.Point.Y)
                {
                    var lineToAdd = new Line()
                    {
                        X1 = edge.Source.Point.X + (edge.Source.Point.X > edge.Target.Point.X ? -0.2 : 0.2),
                        Y1 = edge.Source.Point.Y,
                        X2 = edge.Target.Point.X - (edge.Source.Point.X > edge.Target.Point.X ? -0.2 : 0.2),
                        Y2 = edge.Target.Point.Y,
                        Stroke = stroke,
                        StrokeThickness = strokeThickness
                    };

                    Area.AddCustomChildControl(lineToAdd);
                }

                //var textBlock = new TextBlock()
                //{
                //    Text = edge.Source.Point.X.ToString("F2") + " ; " + edge.Source.Point.Y.ToString("F2"),
                //    FontSize = 2
                //};

                //var sourceCircle = new Ellipse()
                //{
                //    Width = 4,
                //    Height = 4,
                //    Stroke = Brushes.Black,
                //    StrokeThickness = 1
                //};
                //sourceCircle.ToolTip = edge.Source.Point.X.ToString("F2") + " ; " + edge.Source.Point.Y.ToString("F2");
                //Area.AddCustomChildControl(sourceCircle);
                //GraphAreaBase.SetX(sourceCircle, edge.Source.Point.X - 2);
                //GraphAreaBase.SetY(sourceCircle, edge.Source.Point.Y - 2);
            }
            var algorithmBaseClass = (Area.LogicCore.ExternalEdgeRoutingAlgorithm as Oer.OrthogonalEdgeRoutingAlgorithm<DataVertex, DataEdge>);
            //foreach (var edge in algorithmBaseClass.Graph.Edges)
            //{
            //    DrawOrthogonalEdge(algorithmBaseClass, edge);
            //}
            return;
        }

        
        
    }
    
}
