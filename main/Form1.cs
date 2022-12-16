using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.TreeView;

namespace main
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();

            this.BackColor = Color.White;
            this.Text = "Paple";
            this.AutoScroll = true;
            this.Width = 1200;
            this.Height = 680;

            t.TextChanged += ShowText;
            t.Multiline = true;
            t.ScrollBars = ScrollBars.Vertical;
            t.Width = 1000;
            t.Height = 150;

            b.Click += Perform;
            b.Width = 100;
            b.Height = 150;
            b.Text = "Enter";

            l.ReadOnly = true;
            l.Multiline = true;
            l.ScrollBars = ScrollBars.Vertical;
            l.Width = 1120;
            l.Height = 150;

            graphArea.Width = 1120;
            graphArea.Height = 500;
            graphArea.SizeMode = PictureBoxSizeMode.AutoSize;
        }

        TextBox t = new TextBox();
        Button b = new Button();
        TextBox l = new TextBox();
        PictureBox graphArea = new PictureBox();

        string graphStr =
                "100_100 | 200_250 |}" +
                "100_100 | 250_80  |}" +
                "200_250 | 250_80  |}" +
                "200_250 | 400_200 |}" +
                "250_80  | 400_200 |}" +
                "200_250 | 450_300 |}" +
                "250_80  | 500_100 |}" +
                "400_200 | 500_100 |}" +
                "500_100 | 550_200 |}" +
                "400_200 | 550_200 |}" +
                "450_300 | 550_200";

        int marginTop = 10;
        int marginLeft = 10;

        string instruction;
        string[,] graph;
             
        void Perform(object sender, EventArgs e)
        {
            string[] methodsMas = instruction.Split(')');
            string info = "";

            foreach (string method in methodsMas)
            {
                method.Replace("\n", "");

                if (method.Trim() != "")
                {
                    string methodName = method.Split('(')[0].Trim();
                    string methodContent = method.Split('(').Length == 1? 
                        "" : method.Split('(')[1].Trim();

                    switch (methodName)
                    {
                        case "createGraph":
                            GraphMethod(methodContent);
                            break;
                        case "pointList":
                            info += graph != null?
                                "Point list: " + PointListStr(GetPointLst(graph)) +
                                Environment.NewLine +
                                Environment.NewLine : "Graph not initialiced";
                            break;
                        case "numPoints":
                            info += graph != null?
                                "Number of points: " + GetPointLst(graph).Length +
                                Environment.NewLine +
                                Environment.NewLine : "Graph not initialiced";
                            break;
                        case "lineList":
                            info += graph != null?
                                "Line list: " + LineStr(graph) +
                                Environment.NewLine +
                                Environment.NewLine : "Graph not initialiced";
                            break;
                        case "numLines":
                            info += graph != null?
                                "Number of lines: " + (graph.GetUpperBound(0) + 1) +
                                Environment.NewLine +
                                Environment.NewLine : "Graph not initialiced";
                            break;
                        case "adjacencyMatrix":
                            info += graph != null?
                                "Adjacency matrix: " + 
                                Environment.NewLine +
                                showAdjacencyMatrix(createAdjacencyMatrix(graph)) + 
                                Environment.NewLine : "Graph not initialiced";
                            break;
                        case "shortestPath":
                            if (graph != null && methodContent.Split(',').Length > 1)
                            {
                                info += "Shortest path: " +
                                shortestPathStr(shortestPathLst(graph,
                                methodContent.Split(',')[0].Trim(),    
                                methodContent.Split(',')[1].Trim()));
                            }
                            else info += "Path not initialised";

                            info += Environment.NewLine + Environment.NewLine;
                            
                            break;
                        case "default":
                            info += defaultInstructs();
                            break;
                        case "defaultGraph":
                            GraphMethod(graphStr);
                            break;
                        case "randomGraph":
                            if (methodContent.Split(',').Length > 1)
                            {
                                DrawRandomGraph(
                                    Convert.ToInt32(methodContent.Split(',')[0]),
                                    Convert.ToInt32(methodContent.Split(',')[1])
                                );
                            }
                            else info += "Graph not initialiced";

                            info += Environment.NewLine + Environment.NewLine;

                            break;
                        default:
                            info += "Incorrect Method";
                            break;
                    }
                }
            }
            l.Text = info;
        }

        void ShowText(object sender, EventArgs e)
        {
            instruction = t.Text;
        }

        void GraphMethod(string instruction)
        {
            graph = CreateGraph(instruction);

            DrawGraph(graph);
        }

        string[,] CreateGraph(string instruction)
        {
            string[] lines = instruction.Split('}');
            string[,] graph = new string[lines.Length, 2];

            for (int i = 0; i < lines.Length; i++)
            {
                string[] points = lines[i].Split('|');

                graph[i, 0] = points[0].Trim();
                graph[i, 1] = points[1].Trim();
            }
            return graph;
        }

        void DrawGraph(string[,] graph)
        {
            Graphics g = graphArea.CreateGraphics();
            g.Clear(Color.White);

            DrawLine("0_0", "100_0");
            DrawLine("0_0", "0_100");

            for (int i = 0; i < graph.GetUpperBound(0) + 1; i++)
            {
                DrawPoint(graph[i, 0], "black");
                DrawPoint(graph[i, 1], "black");
                DrawLine(graph[i, 0], graph[i, 1]);
            }

            void DrawPoint(string point, string color)
            {
                g.DrawEllipse(Pens.Black,
                    int.Parse(point.Split('_')[0]) - 3,
                    int.Parse(point.Split('_')[1]) - 3, 
                    6, 6
                );
            }
            void DrawLine(string point1, string point2)
            {
                g.DrawLine(Pens.Black, 
                    int.Parse(point1.Split('_')[0]),
                    int.Parse(point1.Split('_')[1]),
                    int.Parse(point2.Split('_')[0]),
                    int.Parse(point2.Split('_')[1])
                );
            }
        }

        string[] GetPointLst(string[,] graph)
        {
            string[] pointLst = { graph[0, 0] };

            foreach (string point1 in graph)
            {
                bool addPoint = true;

                foreach (string point2 in pointLst)
                {
                    if (point1 == point2) addPoint = false;
                }
                if (!addPoint) continue;

                string[] tempMas = new string[pointLst.Length + 1];
                pointLst.CopyTo(tempMas, 0);
                tempMas[pointLst.Length] = point1;
                pointLst = tempMas;
            }

            return pointLst;
        }

        string PointListStr(string[] pointLst)
        {
            string pointStr = "";

            for (int i = 0; i < pointLst.Length; i++)
            {
                pointStr += "[ ";
                pointStr += int.Parse(pointLst[i].Split('_')[0]) + ", ";
                pointStr += int.Parse(pointLst[i].Split('_')[1]);
                pointStr += " ]";

                if (i < pointLst.Length - 1)
                {
                    pointStr += ", ";
                }
            }

            return "[ " + pointStr + " ]";
        }

        string LineStr(string[,] graph)
        {
            string result = "";

            for (int i = 0; i < graph.GetUpperBound(0) + 1; i++)
            {
                result += "[ [ ";
                result += int.Parse(graph[i, 0].Split('_')[0]) + ", ";
                result += int.Parse(graph[i, 0].Split('_')[1]);
                result += " ], [ ";
                result += int.Parse(graph[i, 1].Split('_')[0]) + ", ";
                result += int.Parse(graph[i, 1].Split('_')[1]);
                result += " ] ]";

                if (i < graph.GetUpperBound(0))
                {
                    result += ", ";
                }
            }

            return result;
        }

        string[,] createAdjacencyMatrix(string[,] graph)
        {
            string[] pointLst = GetPointLst(graph);
            string[,] matrix = new string[pointLst.Length + 1, pointLst.Length + 1];
            matrix[0, 0] = "Points";

            for (int i = 0; i < pointLst.Length; i++)
            {
                for (int j = 0; j < pointLst.Length; j++)
                {
                    for (int k = 0; k < graph.GetUpperBound(0) + 1; k++)
                    {
                        if (
                            (pointLst[i] == graph[k, 0] && pointLst[j] == graph[k, 1]) ||
                            (pointLst[i] == graph[k, 1] && pointLst[j] == graph[k, 0])
                        )
                        {
                            matrix[i + 1, j + 1] = "1";
                            break;
                        }
                        else matrix[i + 1, j + 1] = "0";
                    }
                }

                matrix[0, i + 1] = pointLst[i];
                matrix[i + 1, 0] = pointLst[i];
            }

            return matrix;
        }

        string showAdjacencyMatrix(string[,] matrix)
        {
            string matrixStr = "";

            for (int i = 0; i < matrix.GetUpperBound(0) + 1; i++)
            {
                for (int j = 0; j < matrix.GetUpperBound(1) + 1; j++)
                {
                    matrixStr += "\t" + matrix[i, j];
                }

                matrixStr += Environment.NewLine;
            }

            return matrixStr;
        }

        string[] shortestPathLst(string[,] graph, string point1, string point2)
        {
            string[] finalPath = { };
            string[,] matrix = createAdjacencyMatrix(graph);
            string[] pointLst = new string[matrix.GetUpperBound(1)];

            for (int i = 0; i < matrix.GetUpperBound(1); i++)
            {
                if (matrix[i + 1, 0] != point1)
                {
                    pointLst[i] = matrix[i + 1, 0] + ",-1";
                }
                else
                {
                    pointLst[i] = matrix[i + 1, 0] + ",0";
                }
            }

            string[] pathLst = { point1 };
            int index = 0;

            while (index < pathLst.Length)
            {
                string currentPath = pathLst[index];
                string[] currentPathLst = currentPath.Split(',');
                string currentPoint = currentPathLst[currentPathLst.Length - 1];
                bool updateInd = false;
                int currentInd = 0;

                for (; currentInd < pointLst.Length; currentInd++)
                {
                    if (pointLst[currentInd].Split(',')[0] == currentPoint) break;
                }

                for (int i = 0; i < pointLst.Length; i++)
                {
                    if (matrix[currentInd + 1, i + 1] == "1")
                    {
                        double lineWeigth = Math.Sqrt(
                                Math.Pow(
                                    Convert.ToInt32(currentPoint.Split('_')[0]) -
                                    Convert.ToInt32(pointLst[i].Split(',')[0].Split('_')[0]),
                                2.0) +
                                Math.Pow(
                                    Convert.ToInt32(currentPoint.Split('_')[1]) -
                                    Convert.ToInt32(pointLst[i].Split(',')[0].Split('_')[1]),
                                2.0)
                            );

                        if (pointLst[i].Split(',')[1] == "-1")
                        {
                            string tempStr = pointLst[i].Split(',')[0] + "," +
                                (Convert.ToDouble(pointLst[currentInd].Split(',')[1]) + lineWeigth);
                            pointLst[i] = tempStr;

                            string[] tempMas = new string[pathLst.Length + 1];
                            pathLst.CopyTo(tempMas, 0);
                            tempMas[pathLst.Length] = 
                                currentPath + "," + pointLst[i].Split(',')[0];
                            pathLst = tempMas;

                            updateInd = true;
                        }

                        if (lineWeigth + Convert.ToDouble(pointLst[currentInd].Split(',')[1])
                            < Convert.ToDouble(pointLst[i].Split(',')[1]))
                        {
                            for (int j = 0; j < pathLst.Length; j++)
                            {
                                if (pathLst[j].Contains(pointLst[i].Split(',')[0]))
                                {
                                    string[] tempPath = pathLst[j].Split(',');
                                    string tempPathStr = "";

                                    for (int k = 0; k < tempPath.Length - 1; k++)
                                    {
                                        tempPathStr += tempPath[k];

                                        if (k < tempPath.Length - 2) tempPathStr += ",";
                                    }

                                    pathLst[j] = tempPathStr;
                                }
                            }

                            string tempStr = pointLst[i].Split(',')[0] + "," +
                                (Convert.ToDouble(pointLst[currentInd].Split(',')[1]) + lineWeigth);
                            pointLst[i] = tempStr;

                            string[] tempMas = new string[pathLst.Length + 1];
                            pathLst.CopyTo(tempMas, 0);
                            tempMas[pathLst.Length] = 
                                currentPath + "," + pointLst[i].Split(',')[0];
                            pathLst = tempMas;

                            updateInd = true;
                        }
                    }
                }

                if (updateInd)
                {
                    string[] tempMas1 = new string[pathLst.Length - 1];

                    for (int i = 0; i < pathLst.Length - 1; i++)
                    {
                        if (i < index) tempMas1[i] = pathLst[i];
                        else tempMas1[i] = pathLst[i + 1];
                    }

                    pathLst = tempMas1;
                    index = 0;
                }
                else index++;
            }

            for(int i = 0; i < pathLst.Length; i++)
            {
                if (pathLst[i].Contains(point2))
                {
                    finalPath = pathLst[i].Split(',');
                    break;
                }
            }

            return finalPath;
        }

        string shortestPathStr(string[] path)
        {
            string pathStr = "";

            Graphics g = graphArea.CreateGraphics();

            for (int i = 0; i < path.Length; i++)
            {
                pathStr += path[i];

                if (i < path.Length - 1) pathStr += ", ";

                g.DrawEllipse(Pens.Red,
                   int.Parse(path[i].Split('_')[0]) - 5,
                   int.Parse(path[i].Split('_')[1]) - 5,
                10, 10);
            }

            return "[ " + pathStr + " ]";
        }

        void DrawRandomGraph(int numPoints, int numLines)
        {
            Random rand = new Random();

            int maxNumLines = numPoints * (numPoints - 1) / 2;

            if (numLines > maxNumLines) numLines = maxNumLines;

            string[] pointLst = { };

            for (int i = 0; i < numPoints; i++)
            {
                string[] tempMas = new string[pointLst.Length + 1];
                pointLst.CopyTo(tempMas, 0);
                tempMas[pointLst.Length] = 
                    (rand.Next(0, 1101) + 10) + "_" + (rand.Next(0, 481) + 10);
                pointLst = tempMas;
            }

            string[] variantsLst = { };

            for (int i = 0; i < pointLst.Length; i++)
            {
                for (int j = 0; j < pointLst.Length; j++)
                {
                    if (i > j)
                    {
                        string[] tempMas = new string[variantsLst.Length + 1];
                        variantsLst.CopyTo(tempMas, 0);
                        tempMas[variantsLst.Length] = pointLst[i] + "," + pointLst[j];
                        variantsLst = tempMas;
                    }
                }
            }

            string[] lineList = { };

            for (int i = 0; i < numLines; i++)
            {
                int ind = rand.Next(0, variantsLst.Length);

                string[] tempMas = new string[lineList.Length + 1];
                lineList.CopyTo(tempMas, 0);
                tempMas[lineList.Length] = variantsLst[ind];
                lineList = tempMas;

                tempMas = new string[variantsLst.Length - 1];

                for (int j = 0; j < variantsLst.Length - 1; j++)
                {
                    if (j < ind) tempMas[j] = variantsLst[j];
                    else tempMas[j] = variantsLst[j + 1];
                }

                variantsLst = tempMas;
            }

            string[,] tempGraph = new string[lineList.Length, 2];

            for (int i = 0; i < lineList.Length; i++)
            {
                tempGraph[i, 0] = lineList[i].Split(',')[0];
                tempGraph[i, 1] = lineList[i].Split(',')[1];
            }

            DrawGraph(tempGraph);

            graph = tempGraph;
        }

        string defaultInstructs()
        {
            string instructions = "";

            GraphMethod(graphStr);

            instructions += "Shortest path: " +
            shortestPathStr(shortestPathLst(graph, "100_100", "550_200")) +
            Environment.NewLine +
            Environment.NewLine; 

            instructions += "Adjacency matrix: " +
            Environment.NewLine +   
            showAdjacencyMatrix(createAdjacencyMatrix(graph)) +
            Environment.NewLine +
            Environment.NewLine;

            instructions += "Point list: " + 
            PointListStr(GetPointLst(graph)) +
            Environment.NewLine +
            Environment.NewLine;

            instructions += "Number of points: " +
            GetPointLst(graph).Length +
            Environment.NewLine +
            Environment.NewLine;

            instructions += "Line list: " +
            LineStr(graph) +
            Environment.NewLine +
            Environment.NewLine;

            instructions += "Number of lines: " +
            (graph.GetUpperBound(0) + 1) +
            Environment.NewLine +
            Environment.NewLine;

            return instructions;
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            t.Location = new Point(marginLeft, marginTop);
            this.Controls.Add(t);
            marginTop += 160;

            b.Location = new Point(1020, 10);
            this.Controls.Add(b);

            graphArea.Location = new Point(marginLeft, marginTop);
            this.Controls.Add(graphArea);
            marginTop += 510;

            l.Location = new Point(marginLeft, marginTop);
            this.Controls.Add(l);
            marginTop += 150;

            Label scrollLabel = new Label();
            scrollLabel.Location = new Point(marginLeft, marginTop);
            this.Controls.Add(scrollLabel);
        }
    }
}