using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Reflection;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Xml.Linq;
using System.Xml.Serialization;
using IShapeContract;

namespace Paint
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        ShapeFactory _factory;
        List<SolidColorBrush> _brushes = new List<SolidColorBrush>()
        {
            Brushes.Black,
            Brushes.Red,
            Brushes.Green,
            Brushes.Blue,
            Brushes.Yellow,
            Brushes.White,
            Brushes.Pink,
            Brushes.Orange,
            Brushes.Purple,
            Brushes.Gray,
            Brushes.Brown,
            Brushes.Cyan
        };

        List<DoubleCollection> _strokeTypes = new List<DoubleCollection>()
        {
            new DoubleCollection(Array.Empty<double>()),
            new DoubleCollection(new double[] { 1 }),
            new DoubleCollection(new double[] { 3 }),
            new DoubleCollection(new double[] { 3,1,1,1 }),
            new DoubleCollection(new double[] { 3,1,1,1,1,1 }),
        };

        SolidColorBrush _brush;
        int _thickness = 1;
        DoubleCollection _strokeType;

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            var abilities = new List<IShape>();

            // Do tim cac kha nang
            string folder = AppDomain.CurrentDomain.BaseDirectory;
            var fis = (new DirectoryInfo(folder)).GetFiles("*.dll");

            foreach (var fi in fis)
            {
                var assembly = Assembly.LoadFrom(fi.FullName);
                var types = assembly.GetTypes();

                foreach (var type in types)
                {
                    if (type.IsClass & (!type.IsAbstract))
                    {
                        if (typeof(IShape).IsAssignableFrom(type))
                        {
                            if (!typeof(ImageShape).IsAssignableFrom(type))
                            {
                                var shape = Activator.CreateInstance(type) as IShape;

                                abilities.Add(shape!);
                            }
                        }
                    }
                }
            }

            _factory = new ShapeFactory();
            foreach (var ability in abilities)
            {
                _factory.Prototypes.Add(
                    ability.Name, ability
                );

                var button = new Button()
                {
                    Width = 80,
                    Height = 35,
                    Content = ability.Name,
                    Tag = ability.Name
                };
                button.Click += (sender, args) =>
                {
                    var control = (Button)sender;
                    _choice = (string)control.Tag;
                };
                actionsStackPanel.Children.Add(button);
            };

            foreach (var brush in _brushes)
            {
                var button = new Button()
                {
                    Width = 20,
                    Height = 20,
                    Background = brush
                };
                button.Click += (sender, args) =>
                {
                    var control = (Button)sender;
                    _brush = (SolidColorBrush)control.Background;
                    lblCurrentColor.Background = _brush;
                };

                colorsStackPanel.Children.Add(button);
            };

            foreach (var strokeType in _strokeTypes)
            {
                var button = new Button()
                {
                    Width = 60,
                    Height = 40,
                    Content = new Line()
                    {
                        X1 = 0,
                        Y1 = 10,
                        X2 = 50,
                        Y2 = 10,
                        Stroke = Brushes.Black,
                        StrokeThickness = 2,
                        StrokeDashArray = strokeType
                    }
                };
                button.Click += (sender, args) =>
                {
                    var control = (Button)sender;
                    var rectangle = (Line)control.Content;
                    _strokeType = rectangle.StrokeDashArray;
                };

                typeStackPanel.Children.Add(button);
            };

            if (abilities.Count > 0)
            {
                _choice = abilities[0].Name;
                _brush = _brushes[0];
                _thickness = (int)sliderPenWidth.Value;
                _strokeType = _strokeTypes[0];
            }

            btnAddLayer_Click(null, null);

            currentLayer = _layerManager.Layers[0];
            currentLayer.GetBitmap();
            //Binding layerManager to listview
            listViewLayers.ItemsSource = _layerManager.Layers;
            listViewLayers.SelectedItem = currentLayer;

            Canvas.SetZIndex(drawingCanvas, 0);
            Canvas.SetZIndex(touchingCanvas, 1);
            Canvas.SetZIndex(EdittingCanvas, 1);

            transformGroup.Children.Add(scale);
            transformGroup.Children.Add(translate);

            //Gắn event cho các canvas để zoom
            drawingCanvas.LayoutTransform = transformGroup;
            drawingCanvas.MouseWheel += Canvas_MouseWheel;

            EdittingCanvas.LayoutTransform = transformGroup;
            EdittingCanvas.MouseWheel += Canvas_MouseWheel;
            
            touchingCanvas.LayoutTransform = transformGroup;
            touchingCanvas.MouseWheel += Canvas_MouseWheel;
            

            StartRectangle.MouseDown += (sender, args) =>
            {
                isResizing = true;
                dataUndoRedoForEditting = new ShapeUndoRedo()
                {
                    CurrentLayer = currentLayer,
                    OldShape = shapeEditting.Clone(),
                    IndexInLayer = currentLayer.Shapes.IndexOf(shapeEditting),
                    TypeOfData = ShapeUndoRedo.Type.Change
                };
                chosenResizeRec = StartRectangle;
            };

            StartRectangle.MouseUp += (sender, args) =>
            {
                isResizing = false;
                if(dataUndoRedoForEditting != null)
                {
                    dataUndoRedoForEditting.NewShape = shapeEditting.Clone();
                    undoRedoManager.AddUndoRedo(dataUndoRedoForEditting);
                    dataUndoRedoForEditting = null;
                }
                currentLayer.GetBitmap();
            };

            EndRectangle.MouseDown += (sender, args) =>
            {
                isResizing = true;
                dataUndoRedoForEditting = new ShapeUndoRedo()
                {
                    CurrentLayer = currentLayer,
                    OldShape = shapeEditting.Clone(),
                    IndexInLayer = currentLayer.Shapes.IndexOf(shapeEditting),
                    TypeOfData = ShapeUndoRedo.Type.Change
                };
                chosenResizeRec = EndRectangle;
            };

            EndRectangle.MouseUp += (sender, args) =>
            {
                isResizing = false;
                if(dataUndoRedoForEditting != null)
                {
                    dataUndoRedoForEditting.NewShape = shapeEditting.Clone();
                    undoRedoManager.AddUndoRedo(dataUndoRedoForEditting);
                    dataUndoRedoForEditting = null;
                }
                currentLayer.GetBitmap();
            };

            MoveIcon.MouseDown += (sender, args) =>
            {
                isMoving = true;
                dataUndoRedoForEditting = new ShapeUndoRedo()
                {
                    CurrentLayer = currentLayer,
                    OldShape = shapeEditting.Clone(),
                    IndexInLayer = currentLayer.Shapes.IndexOf(shapeEditting),
                    TypeOfData = ShapeUndoRedo.Type.Change
                };
                chosenResizeRec = MoveIcon;
            };

            MoveIcon.MouseUp += (sender, args) =>
            {
                isMoving = false;
                if(dataUndoRedoForEditting != null)
                {
                    dataUndoRedoForEditting.NewShape = shapeEditting.Clone();
                    undoRedoManager.AddUndoRedo(dataUndoRedoForEditting);
                    dataUndoRedoForEditting = null;
                }
                currentLayer.GetBitmap();
            };
        }

        //các biến lưu trạng thái
        bool isDrawing = false;
        System.Windows.Point _start;
        System.Windows.Point _end;
        string _choice; // Line
        List<IShape> _shapes = new List<IShape>();
        LayerManager _layerManager = new LayerManager();
        Layer currentLayer;
        int currentLayerIndex = 0;

        ScaleTransform scale = new ScaleTransform();

        // Create a new TranslateTransform
        TranslateTransform translate = new TranslateTransform();

        // Create a new TransformGroup and add the ScaleTransform and TranslateTransform to it
        TransformGroup transformGroup = new TransformGroup();
        
        bool isEditting = false;
        
        IShape shapeEditting;
        IShape shapeCopy;

        private IShape preview;
        private bool isResizing = false;
        private bool isMoving = false;
        private System.Windows.Shapes.Rectangle chosenResizeRec;

        UndoRedoManager undoRedoManager = new UndoRedoManager();
        ShapeUndoRedo dataUndoRedoForEditting;

        
        // Các hàm xử lí sự kiện Edit 
        private void Canvas_MouseDownEditing(object sender, MouseButtonEventArgs e)
        {
            //kiem tra xem co click vao 1 shape nao khong
            Canvas canvas = (Canvas)sender;
            var element = canvas.InputHitTest(e.GetPosition(canvas)) as UIElement;
            if(element == canvas)
            {
                isEditting = false;
                EdittingCanvas.Visibility = Visibility.Collapsed;
            }
        }

        private void Canvas_MouseMoveEditing(object sender, MouseEventArgs e)
        {
            if(isResizing)
            {
                if(chosenResizeRec == StartRectangle)
                {
                    System.Windows.Point mousePosition = e.GetPosition(drawingCanvas);
                    shapeEditting.Points[0] = mousePosition;
                    Canvas.SetLeft(StartRectangle, shapeEditting.Points[0].X - 5);
                    Canvas.SetTop(StartRectangle, shapeEditting.Points[0].Y - 5);

                    Canvas.SetLeft(MoveIcon, (shapeEditting.Points[0].X + shapeEditting.Points[1].X) / 2 - 13);
                    Canvas.SetTop(MoveIcon, (shapeEditting.Points[0].Y + shapeEditting.Points[1].Y) / 2 - 13);
                    LoadAllShapes();
                }
                else if(chosenResizeRec == EndRectangle)
                {
                    System.Windows.Point mousePosition = e.GetPosition(drawingCanvas);
                    shapeEditting.Points[1] = mousePosition;
                    Canvas.SetLeft(EndRectangle, shapeEditting.Points[1].X - 5);
                    Canvas.SetTop(EndRectangle, shapeEditting.Points[1].Y - 5);

                    Canvas.SetLeft(MoveIcon, (shapeEditting.Points[0].X + shapeEditting.Points[1].X) / 2 - 13);
                    Canvas.SetTop(MoveIcon, (shapeEditting.Points[0].Y + shapeEditting.Points[1].Y) / 2 - 13);
                    LoadAllShapes();
                }
                
            }else if (isMoving)
            {
                if (chosenResizeRec == MoveIcon)
                {
                    System.Windows.Point mousePosition = e.GetPosition(drawingCanvas);
                    System.Windows.Point vector = new System.Windows.Point(mousePosition.X - (shapeEditting.Points[0].X + shapeEditting.Points[1].X) / 2, mousePosition.Y - (shapeEditting.Points[0].Y + shapeEditting.Points[1].Y) / 2);
                    shapeEditting.Points[0] = new System.Windows.Point(shapeEditting.Points[0].X + vector.X, shapeEditting.Points[0].Y + vector.Y);
                    shapeEditting.Points[1] = new System.Windows.Point(shapeEditting.Points[1].X + vector.X, shapeEditting.Points[1].Y + vector.Y);
                    Canvas.SetLeft(StartRectangle, shapeEditting.Points[0].X - 5);
                    Canvas.SetTop(StartRectangle, shapeEditting.Points[0].Y - 5);
                    Canvas.SetLeft(EndRectangle, shapeEditting.Points[1].X - 5);
                    Canvas.SetTop(EndRectangle, shapeEditting.Points[1].Y - 5);

                    Canvas.SetLeft(MoveIcon, (shapeEditting.Points[0].X + shapeEditting.Points[1].X) / 2 - 13);
                    Canvas.SetTop(MoveIcon, (shapeEditting.Points[0].Y + shapeEditting.Points[1].Y) / 2 - 13);
                    LoadAllShapes();
                }
            } 
                
        }
     
        //-------------------------------------------------------------------------------------------
        // Các hàm xử lí sự kiện vẽ

        private void Canvas_MouseDown(object sender, MouseButtonEventArgs e)
        {
            isDrawing = true;
            _start = e.GetPosition(drawingCanvas);
        }

        

        private void Canvas_MouseMove(object sender, MouseEventArgs e)
        {
            _end = e.GetPosition(drawingCanvas);
            if (isDrawing)
            {

                Title = $"{_start.X}, {_start.Y} => {_end.X}, {_end.Y}";

                preview = _factory.Create(_choice);

                preview.Brush = _brush;
                preview.Thickness = _thickness;
                preview.DashArray = _strokeType;
                preview.Points.Add(_start);
                preview.Points.Add(_end);


                LoadAllShapes();

                UIElement preUI = preview.Draw();
                preUI.PreviewMouseDown += (sender, args) =>
                {
                    EdittingCanvas.Visibility = Visibility.Visible;
                    listViewLayers.SelectedItem = currentLayer;
                    shapeEditting = preview;
                    isEditting = true;
                    Canvas.SetLeft(StartRectangle, preview.Points[0].X - 5);
                    Canvas.SetTop(StartRectangle, preview.Points[0].Y - 5);
                    Canvas.SetLeft(EndRectangle, preview.Points[1].X - 5);
                    Canvas.SetTop(EndRectangle, preview.Points[1].Y - 5);

                    Canvas.SetLeft(MoveIcon, (preview.Points[0].X + preview.Points[1].X) / 2 - 13);
                    Canvas.SetTop(MoveIcon, (preview.Points[0].Y + preview.Points[1].Y) / 2 - 13);
                };
              
                drawingCanvas.Children.Add(preUI);
                currentLayer._canvas.Children.Add(preview.Draw());
            }
                
        }

        private void LoadAllShapes()
        {
            drawingCanvas.Children.Clear();
            foreach (var layer in _layerManager.Layers)
            {
                layer._canvas.Children.Clear();

                if (!layer.IsVisible)
                {
                    foreach (var shape in layer.Shapes)
                    {
                        layer._canvas.Children.Add(shape.Draw());
                    }
                }
                else
                {
                    foreach (var shape in layer.Shapes)
                    {
                        UIElement ui = shape.Draw();
                        layer._canvas.Children.Add(shape.Draw());
                        ui.PreviewMouseDown += (sender, args) =>
                        {
                            EdittingCanvas.Visibility = Visibility.Visible;
                            listViewLayers.SelectedItem = layer;
                            shapeEditting = shape;
                            isEditting = true;
                            Canvas.SetLeft(StartRectangle, shape.Points[0].X - 5);
                            Canvas.SetTop(StartRectangle, shape.Points[0].Y - 5);
                            Canvas.SetLeft(EndRectangle, shape.Points[1].X - 5);
                            Canvas.SetTop(EndRectangle, shape.Points[1].Y - 5);

                            Canvas.SetLeft(MoveIcon, (shape.Points[0].X + shape.Points[1].X)/2 - 13);
                            Canvas.SetTop(MoveIcon, (shape.Points[0].Y + shape.Points[1].Y)/2 - 13);
                        };
                        
                        drawingCanvas.Children.Add(ui);
                    }
                }
            }
        }

        private void Canvas_MouseUp(object sender, MouseButtonEventArgs e)
        {
            if (preview != null)
            {
                DataUndoRedo data = new ShapeUndoRedo()
                {
                    CurrentLayer = currentLayer,
                    NewShape = preview.Clone(),
                    IndexInLayer = currentLayer.Shapes.Count,
                    TypeOfData = ShapeUndoRedo.Type.Add
                };
                undoRedoManager.AddUndoRedo(data);

                currentLayer.AddShape(preview);
            }
            isDrawing = false;
        }
        //-------------------------------------------------------------------------------------------
       
        // Các hàm xử lí sự kiện Save và Load hình ảnh
        public void SaveCanvases(string filename)
        {
            // Render the canvas to a bitmap
            RenderTargetBitmap renderBitmap = new RenderTargetBitmap(
                (int)drawingCanvas.ActualWidth, (int)drawingCanvas.ActualHeight,
                96d, 96d, PixelFormats.Pbgra32);

            double width = drawingCanvas.ActualWidth;
            double height = drawingCanvas.ActualHeight;
            System.Windows.Size size = new System.Windows.Size(width, height);

            DrawingVisual drawingVisual = new DrawingVisual();
            using (DrawingContext context = drawingVisual.RenderOpen())
            {

                context.DrawRectangle(new VisualBrush(drawingCanvas), null, new Rect(new System.Windows.Point(), size));

            } 

            renderBitmap.Render(drawingVisual);

            // Create a PNG encoder
            PngBitmapEncoder pngImage = new PngBitmapEncoder();
            pngImage.Frames.Add(BitmapFrame.Create(renderBitmap));

            // Save the image to a file
            using (var fs = System.IO.File.OpenWrite(filename))
            {
                pngImage.Save(fs);
            }
        }

        private string OpenSaveFileDialog()
        {
            var dialog = new Microsoft.Win32.SaveFileDialog();
            dialog.Filter = "PNG (*.png)|*.png";
            dialog.InitialDirectory = AppDomain.CurrentDomain.BaseDirectory;
            if (dialog.ShowDialog() == true)
            {
                return dialog.FileName;
            }
            return "";
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            string filename = OpenSaveFileDialog();
            if (filename != "")
            {
                SaveCanvases(filename);
            }
        }

        private string OpenLoadFileDialog()
        {
            var dialog = new Microsoft.Win32.OpenFileDialog();
            dialog.Filter = "PNG (*.png)|*.png";
            dialog.InitialDirectory = AppDomain.CurrentDomain.BaseDirectory;
            if (dialog.ShowDialog() == true)
            {
                return dialog.FileName;
            }
            return "";
        }

        private void LoadImageToLayer(Layer layer, string filename)
        {
            // Create the image element
            Image image = new Image();

            // Create source
            BitmapImage bitmap = new BitmapImage();
            bitmap.BeginInit();
            bitmap.UriSource = new Uri(filename, UriKind.Absolute);
            bitmap.EndInit();
            double factor =  drawingCanvas.ActualWidth / bitmap.Width;

            image.Width = Math.Min( bitmap.Width * factor, bitmap.Width);
            image.Height = Math.Min(bitmap.Height * factor, bitmap.Height);
            image.Stretch = Stretch.Fill;
            // Set the image source
            image.Source = bitmap;
            IShape shape = new ImageShape(bitmap);
            shape.Points.Add(new System.Windows.Point(0, 0));
            shape.Points.Add(new System.Windows.Point(image.Width, image.Height));

            DataUndoRedo data = new ShapeUndoRedo()
            {
                CurrentLayer = layer,
                NewShape = shape.Clone(),
                IndexInLayer = layer.Shapes.Count,
                TypeOfData = ShapeUndoRedo.Type.Add
            };
            undoRedoManager.AddUndoRedo(data);

            layer.AddShape(shape);
            image.MouseDown += (sender, args) =>
            {
                EdittingCanvas.Visibility = Visibility.Visible;
                listViewLayers.SelectedItem = layer;
                shapeEditting = shape;
                isEditting = true;
                Canvas.SetLeft(StartRectangle, shape.Points[0].X - 5);
                Canvas.SetTop(StartRectangle, shape.Points[0].Y - 5);
                Canvas.SetLeft(EndRectangle, shape.Points[1].X - 5);
                Canvas.SetTop(EndRectangle, shape.Points[1].Y - 5);

                Canvas.SetLeft(MoveIcon, (shape.Points[0].X + shape.Points[1].X) / 2 - 13);
                Canvas.SetTop(MoveIcon, (shape.Points[0].Y + shape.Points[1].Y) / 2 - 13);
            };
            // Add Image to Canvas
            drawingCanvas.Children.Add(image);
        }
        
        
        private void btnLoad_Click(object sender, RoutedEventArgs e)
        {
            string filename = OpenLoadFileDialog();
            if (filename != "")
            {
                LoadImageToLayer(currentLayer, filename);
            }
        }


        //-------------------------------------------------------------------------------------------

        // hàm xử lí sự kiện chọn màu
        private void sliderSize_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            lblCurrentPenWidth.Content = "Pen Width: " + (int)sliderPenWidth.Value;
            _thickness = (int)sliderPenWidth.Value;
        }


        //-------------------------------------------------------------------------------------------
        // Các hàm xử lí sự kiện Delete, Copy, Cut, Paste, Undo, Redo, Add, Remove, MoveUp, MoveDown, Hide, Show
        private void btnDelete_Click(object sender, RoutedEventArgs e)
        {
            if(shapeEditting != null)
            {
                if (currentLayer.Shapes.Contains(shapeEditting))
                {
                    DataUndoRedo data = new ShapeUndoRedo()
                    {
                        CurrentLayer = currentLayer,
                        OldShape = shapeEditting.Clone(),
                        IndexInLayer = currentLayer.Shapes.IndexOf(shapeEditting),
                        TypeOfData = ShapeUndoRedo.Type.Remove
                    };
                    undoRedoManager.AddUndoRedo(data);

                    currentLayer.RemoveShape(shapeEditting);
                    LoadAllShapes();
                    EdittingCanvas.Visibility = Visibility.Collapsed;
                    shapeEditting = null;
                }
            }
        }

        private void btnCopy_Click(object sender, RoutedEventArgs e)
        {
            if(shapeEditting != null)
            {
                shapeCopy = shapeEditting.Clone();
            }
        }

        private void btnCut_Click(object sender, RoutedEventArgs e)
        {
            if(shapeEditting != null)
            {
                if (currentLayer.Shapes.Contains(shapeEditting))
                {
                    shapeCopy = shapeEditting.Clone();

                    DataUndoRedo data = new ShapeUndoRedo()
                    {
                        CurrentLayer = currentLayer,
                        OldShape = shapeEditting.Clone(),
                        IndexInLayer = currentLayer.Shapes.IndexOf(shapeEditting),
                        TypeOfData = ShapeUndoRedo.Type.Remove
                    };
                    undoRedoManager.AddUndoRedo(data);

                    currentLayer.RemoveShape(shapeEditting);
                    LoadAllShapes();
                    EdittingCanvas.Visibility = Visibility.Collapsed;
                    shapeEditting = null;
                }
            }
        }

        private void btnPaste_Click(object sender, RoutedEventArgs e)
        {
            if(shapeCopy != null)
            {
                DataUndoRedo data = new ShapeUndoRedo()
                {
                    CurrentLayer = currentLayer,
                    NewShape = shapeCopy.Clone(),
                    IndexInLayer = currentLayer.Shapes.Count,
                    TypeOfData = ShapeUndoRedo.Type.Add
                };
                undoRedoManager.AddUndoRedo(data);

                currentLayer.AddShape(shapeCopy);
                LoadAllShapes();
                EdittingCanvas.Visibility = Visibility.Collapsed;
            }
        }

        private void chboxEdit_Checked(object sender, RoutedEventArgs e)
        {
            touchingCanvas.Visibility = Visibility.Hidden;
            btnDelete.IsEnabled = true;
            btnCopy.IsEnabled = true;
            btnCut.IsEnabled = true;
            btnPaste.IsEnabled = true;

        }

        private void chboxEdit_Unchecked(object sender, RoutedEventArgs e)
        {
            touchingCanvas.Visibility = Visibility.Visible;
            EdittingCanvas.Visibility = Visibility.Collapsed;
            btnDelete.IsEnabled = false;
            btnCopy.IsEnabled = false;
            btnCut.IsEnabled = false;
            btnPaste.IsEnabled = false;

        }

        

        private void btnAddLayer_Click(object sender, RoutedEventArgs e)
        { 
            LayerUndoRedo data = new LayerUndoRedo()
            {
                drawingLayout = drawingLayout,
                layerManager = _layerManager,
                TypeOfData = LayerUndoRedo.Type.Add,
                IndexInLayout = _layerManager.Layers.Count
            };

            _layerManager.AddLayer((int)drawingCanvas.ActualWidth, (int)drawingCanvas.ActualHeight, drawingLayout);
            data.NewLayer = _layerManager.Layers[_layerManager.Layers.Count - 1];
            undoRedoManager.AddUndoRedo(data);
        }

        private void btnRemoveLayer_Click(object sender, RoutedEventArgs e)
        {
            if (_layerManager.Layers.Count == 1)
                return;
            LayerUndoRedo data = new LayerUndoRedo()
            {
                drawingLayout = drawingLayout,
                layerManager = _layerManager,
                TypeOfData = LayerUndoRedo.Type.Remove,
                IndexInLayout = currentLayerIndex,
                OldLayer = currentLayer
            };
            undoRedoManager.AddUndoRedo(data);

            _layerManager.RemoveLayer(currentLayer, drawingLayout);
            listViewLayers.SelectedIndex = 0;
            LoadAllShapes();
        }

        private void btnMoveUpLayer_Click(object sender, RoutedEventArgs e)
        {
            if (currentLayerIndex >= _layerManager.Layers.Count - 1)
                return;
            LayerUndoRedo data = new LayerUndoRedo()
            {
                drawingLayout = drawingLayout,
                layerManager = _layerManager,
                TypeOfData = LayerUndoRedo.Type.MoveUp,
                IndexInLayout = currentLayerIndex + 1,
                OldLayer = currentLayer
            };
            undoRedoManager.AddUndoRedo(data);
            int tempIndex = currentLayerIndex;

            _layerManager.MoveLayerUp(tempIndex);
            listViewLayers.SelectedIndex = tempIndex + 1;
            LoadAllShapes();

        }

        private void btnMoveDownLayer_Click(object sender, RoutedEventArgs e)
        {
            if (currentLayerIndex <= 0)
                return;
            LayerUndoRedo data = new LayerUndoRedo()
            {
                drawingLayout = drawingLayout,
                layerManager = _layerManager,
                TypeOfData = LayerUndoRedo.Type.MoveDown,
                IndexInLayout = currentLayerIndex - 1,
                OldLayer = currentLayer
            };
            undoRedoManager.AddUndoRedo(data);

            int tempIndex = currentLayerIndex;
            _layerManager.MoveLayerDown(tempIndex);
            listViewLayers.SelectedIndex = tempIndex - 1;
            LoadAllShapes();
        }

        private void listViewLayers_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            currentLayer = (Layer)listViewLayers.SelectedItem;
            currentLayerIndex = listViewLayers.SelectedIndex;
            
            if(currentLayerIndex < 0)
                return;
            if (currentLayer.IsVisible)
            {
                btnHideLayer.Content = "Hide";
            }
            else
            { 
                 btnHideLayer.Content = "Show";
            }
            
            if(isEditting)
            {
                isEditting = false;
                EdittingCanvas.Visibility = Visibility.Collapsed;
            }
        }

        private void btnHide_Click(object sender, RoutedEventArgs e)
        {
            if(currentLayer.IsVisible)
            {
                currentLayer.IsVisible = false;
                btnHideLayer.Content = "Show";
            }
            else
            {
                currentLayer.IsVisible = true;
                btnHideLayer.Content = "Hide";
            }
            LoadAllShapes();
        }

        private void Canvas_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            if (Keyboard.IsKeyDown(Key.LeftCtrl) || Keyboard.IsKeyDown(Key.RightCtrl))
            {
                System.Windows.Point mousePosition = e.GetPosition(drawingCanvas);

                

                // Calculate the zoom factor (you can adjust this to get the desired zoom speed)
                double zoomFactor = e.Delta > 0 ? 1.1 : 1 / 1.1;

                // Update the ScaleTransform
                scale.ScaleX *= zoomFactor;
                scale.ScaleY *= zoomFactor;

                // Update the TranslateTransform to keep the mouse pointer at the same position on the Canvas
                translate.X = (1 - zoomFactor) * (mousePosition.X + translate.X);
                translate.Y = (1 - zoomFactor) * (mousePosition.Y + translate.Y);

            }

        }

        private void btnUndo_Click(object sender, RoutedEventArgs e)
        {
            undoRedoManager.Undo();
            LoadAllShapes();
            listViewLayers.SelectedIndex = 0;
            
            EdittingCanvas.Visibility = Visibility.Collapsed;

        }

        private void btnRedo_Click(object sender, RoutedEventArgs e)
        {
            undoRedoManager.Redo();
            LoadAllShapes();
            listViewLayers.SelectedIndex = 0;
            EdittingCanvas.Visibility = Visibility.Collapsed;
        }
    }
}