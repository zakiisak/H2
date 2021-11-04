using FlaskeAutomaten.Items;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace FlaskeAutomaten
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        //The main object where all the magic happens
        public VendingMachine VendingMachine { get; private set; }

        //The lists, handling the beverage labels on the canvas
        private List<BeverageUi> producedBelt = new List<BeverageUi>();
        private List<BeverageUi>[] consumerBelt;

        //Used to know how fast to move the labels
        private double ProducerBarWidth;
        private double ConsumerBarWidth = 200;

        //the move delay variables (used for moving the beverage labels on the screen a specific amount of times per second)
        private int MoveDelay = 16;
        private double MoveDelta =  1.0 / (1000.0 / 16.0);


        public MainWindow()
        {
            InitializeComponent();
            this.ProducerBarWidth = ProducerBar.Width;
            VendingMachine = new VendingMachine();
            InitConsumerBelts();
            CreateSplitterBuffers();
            VendingMachine.Producer.OnProduced = OnVendingProduced;
            VendingMachine.Splitter.OnSplit = OnBeverageSplit;

            for(int i = 0; i < VendingMachine.Consumers.Length; i++)
            {
                int index = i;
                VendingMachine.Consumers[i].OnConsumed = delegate (Beverage consumedBeverage)
                {
                    OnBeverageConsumed(index, consumedBeverage);
                };
            }

            this.Closing += MainWindow_Closing;
        }

        //Called every time a beverage is consumed
        private void OnBeverageConsumed(int consumerIndex, Beverage beverage)
        {
            //Call 
            Dispatcher.Invoke(() =>
            {
                lock(consumerBelt)
                {
                    List<BeverageUi> belt = consumerBelt[consumerIndex];
                    for(int i = 0; i < belt.Count; i++)
                    {
                        BeverageUi ui = belt[i];
                        if(ui.Beverage.Id == beverage.Id)
                        {
                            belt.RemoveAt(i);
                            this.VendingMachineCanvas.Children.Remove(ui.attachedLabel);
                            break;
                        }
                    }
                }
            });
        }

        //An event that fires when the window is about to close.
        //We use this to shut down the vending machine
        private void MainWindow_Closing(object? sender, System.ComponentModel.CancelEventArgs e)
        {
            VendingMachine.TurnOff();
            //Wait 500 ms to help it close all the threads properly (Dispatcher exceptions could otherwise occur)
            Thread.Sleep(500);
        }

        //Initializes The lists handling the beverages on the consumer belts
        private void InitConsumerBelts()
        {
            consumerBelt = new List<BeverageUi>[this.VendingMachine.Consumers.Length];
            for (int i = 0; i < consumerBelt.Length; i++)
                consumerBelt[i] = new List<BeverageUi>();
        }

        //The main function to move the labels on the canvas
        private void MoveLabels(object? state)
        {
            while(VendingMachine.IsOn)
            {
                //Run on the ui thread
                Dispatcher.Invoke(() =>
                {
                    lock (this.producedBelt)
                    {
                        for (int i = 0; i < this.producedBelt.Count; i++)
                        {
                            BeverageUi ui = this.producedBelt[i];
                            ui.MoveLabel();
                        }
                    }

                    lock(this.consumerBelt)
                    {
                        foreach(List<BeverageUi> belts in this.consumerBelt)
                        {
                            for(int i = 0; i < belts.Count; i++)
                            {
                                BeverageUi ui = belts[i];
                                ui.MoveLabel();
                            }
                        }
                    }
                });
                Thread.Sleep(MoveDelay);
            }
        }

        //Called every time a beverage is produced
        private void OnVendingProduced(Beverage beverage)
        {
            //Run on the ui thread
            Dispatcher.Invoke(() => {

                lock(producedBelt)
                {
                    double secondsDelay = (double)this.VendingMachine.Splitter.Delay / 1000.0;
                    double widthSpand = ProducerBarWidth - beverage.Name.Length * 10;
                    double dx = widthSpand / (secondsDelay + producedBelt.Count) * MoveDelta;
                    BeverageUi ui = GenerateBeverageUi(beverage, 75, 208, dx);
                    producedBelt.Add(ui);
                }
            });
        }

        //An event that fires every time a beverage is split into it's own belt
        private void OnBeverageSplit(Beverage beverage)
        {
            BeverageUi? beverageUi = null;
            lock (producedBelt)
            {
                beverageUi = producedBelt.Find(x => x.Beverage.Id == beverage.Id);
            }
            if(beverageUi != null)
            {
                Dispatcher.Invoke(() =>
                {
                    VendingMachineCanvas.Children.Remove(beverageUi.attachedLabel);
                });
                producedBelt.Remove(beverageUi);

                    
                //find the index that we need to access on the customer belt
                for(int i = 0; i < this.VendingMachine.Consumers.Length; i++)
                {
                    if(this.VendingMachine.Consumers[i].BufferType.Equals(beverage.GetType()))
                    {
                            
                        Dispatcher.Invoke(() =>
                        {
                            lock(consumerBelt)
                            {
                                BeverageUi newBeverageUi = AddConsumerBeverageUi(beverage, i);
                                this.consumerBelt[i].Add(newBeverageUi);
                            }
                        });
                    }
                }
            }
            
        }

        //Fired when the Start button is pressed
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (this.VendingMachine.IsOn)
            {
                this.VendingMachine.TurnOff();
                this.StartButton.Content = "Start";
            }
            else
            {
                this.VendingMachine.TurnOn();
                this.StartButton.Content = "Stop";
                ThreadPool.QueueUserWorkItem(MoveLabels);
            }
        }

        #region UI Methods
        private BeverageUi GenerateBeverageUi(Beverage beverage, double x, double y, double dx)
        {
            Label label = CreateLabel(x, y, beverage.Name);
            BeverageUi ui = new BeverageUi(beverage, label, x, y, dx);
            return ui;
        }

        private static double padding = 30, height = 20, wholeEntryHeight = padding + height;

        //Generates a label on the specific consumer belt, specified by the consumerIndex (points to the VendingMachine.Consumers array)
        //It estimates it's speed depending on the delay of consumption, the width of the consumer belt, and the width of the label.
        private BeverageUi AddConsumerBeverageUi(Beverage beverage, int consumerIndex)
        {
            int count = VendingMachine.Consumers.Length;
            double x = 500;
            double y = this.Height / 2 - (count / 2 * wholeEntryHeight) + consumerIndex * wholeEntryHeight;

            double secondsDelay = (double)this.VendingMachine.Consumers[consumerIndex].ConsumeDelay / 1000.0;
            double widthSpand = ConsumerBarWidth - beverage.Name.Length * 10;
            double dx = widthSpand / (secondsDelay + consumerBelt[consumerIndex].Count) * MoveDelta;

            return GenerateBeverageUi(beverage, x, y, dx);
        }

        //This is where the generation of the consumer belts/lines and so forth are done, depending on the beverage types found in the vending machine
        private void CreateSplitterBuffers()
        {
            int count = VendingMachine.Consumers.Length;

            double yStart = this.Height / 2 - (count / 2 * wholeEntryHeight);
            double x = 500;
            for(int i = 0; i < VendingMachine.Consumers.Length; i++)
            {
                double y = yStart + i * wholeEntryHeight;
                Rectangle rect = CreateRectangle(x, y, ConsumerBarWidth, height, Color.FromRgb(220, 220, 200));
                Label label = CreateLabel(x, y - 20, VendingMachine.Consumers[i].BufferType.Name + " Consumer");
                CreateLine(423, 210, x, y + height / 2, Color.FromRgb(0, 0, 0));

            }
        }

        //Generates a rectangle ui object on the canvas
        private Rectangle CreateRectangle(double x, double y, double width, double height, Color color)
        {
            Rectangle l = new Rectangle();
            l.Width = width;
            l.Height = height;
            l.Fill = new SolidColorBrush(color);
            VendingMachineCanvas.Children.Add(l);
            Canvas.SetTop(l, y);
            Canvas.SetLeft(l, x);
            return l;
        }

        //Generates a label ui object on the canvas
        private Label CreateLabel(double x, double y, string Text)
        {
            Label label = new Label();
            label.Content = Text;
            VendingMachineCanvas.Children.Add(label);
            Canvas.SetTop(label, y);
            Canvas.SetLeft(label, x);
            return label;
        }

        //Generates a line ui object on the canvas
        private Line CreateLine(double x, double y, double x2, double y2, Color color)
        {
            Line l = new Line();
            l.X1 = x;
            l.Y1 = y;
            l.X2 = x2;
            l.Y2 = y2;
            l.Stroke = new SolidColorBrush(color);
            VendingMachineCanvas.Children.Add(l);
            return l;
        }

        #endregion

    }
}
