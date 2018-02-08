using SableFin.LightJ2.CoreCommon.DataModel;
using SableFin.LightJ2.DmxGenerators;
using SableFin.LightJ2.LightJConsole.SimpleArtnet;
using SableFin.LightJ2.LightJConsole.SurfaceControls;
using SableFin.LightJ2.Network;
using SableFin.LightJ2.SurfaceControls;
using SableFin.LightJ2.SurfaceFramework;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using System.Xml.Linq;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace SableFin.LightJ2.LightJConsole
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        public MainPage()
        {
            this.InitializeComponent();
        }


        /// <summary>
        /// Invoked when this page is about to be displayed in a Frame.
        /// </summary>
        /// <param name="e">Event data that describes how this page was reached.  The Parameter
        /// property is typically used to configure the page.</param>
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
        }

        private FixtureDatabase fixturesDb = null;


        private async void Page_Loaded(object sender, RoutedEventArgs e)
        {
            fixturesDb = await LoadFixtureDatabase();
            await LoadConsoleSurfaceConfiguration();
            await LoadSurfaceToFixtureMapping();


            ITimerDmxGenerator g;
            //g=new SinusDmxGenerator(20,230);
            //g.SetBpm(60);
            //g.IsActive = true;
            //TimeDmxClockEngine.Current.AddGenerator("scanPan", g);

            //g = new CosinusDmxGenerator(20, 230);
            //g.SetBpm(60);
            //g.IsActive = true;
            //TimeDmxClockEngine.Current.AddGenerator("scanTilt", g);
            g = new RandomPerBeatGenerator(97,98, 20, 230, 20, 230);
            g.SetBpm(60);
            g.IsActive = true;
            TimeDmxClockEngine.Current.AddGenerator("scanRandom", g);
            TimeDmxClockEngine.Current.SetParameterValue("scanRandom", "lockY", "1");
            TimeDmxClockEngine.Current.SetParameterValue("scanRandom", "lockX", "1");
            //TimeDmxClockEngine.Current.SetParameterValue("scanRandom", "lockGOBO", "1");


            g = new RandomPerBeatGenerator(110, 111, 20, 230, 20, 230);
            g.SetBpm(60);
            g.IsActive = true;
            TimeDmxClockEngine.Current.AddGenerator("scanLyre1", g);
            TimeDmxClockEngine.Current.SetParameterValue("scanLyre1", "lockY", "1");
            TimeDmxClockEngine.Current.SetParameterValue("scanLyre1", "lockX", "1");

            g = new RandomPerBeatGenerator(120, 121, 20, 230, 20, 230);
            g.SetBpm(60);
            g.IsActive = true;
            TimeDmxClockEngine.Current.AddGenerator("scanLyre2", g);
            TimeDmxClockEngine.Current.SetParameterValue("scanLyre2", "lockY", "1");
            TimeDmxClockEngine.Current.SetParameterValue("scanLyre2", "lockX", "1");

            var player1 = new SequencePlayer(@"UwpTestSequence\sequence001.json");
            TimeDmxClockEngine.Current.AddGenerator("seq001", player1);


            var player2 = new SequencePlayer(@"UwpTestSequence\sequence002.json");
            TimeDmxClockEngine.Current.AddGenerator("seq002", player2);

            var player3 = new SequencePlayer(@"UwpTestSequence\sequence003.json");
            TimeDmxClockEngine.Current.AddGenerator("seq003", player3);

            var player4 = new SequencePlayer(@"UwpTestSequence\sequence004.json");
            TimeDmxClockEngine.Current.AddGenerator("seq004", player4);


            TimeDmxClockEngine.Current.SetBpm(60);
            TimeDmxClockEngine.Current.Start();
        }

        async Task<FixtureDatabase> LoadFixtureDatabase()
        {
            var file = await GetPackagedFile("Data", "FixturesDatabase.xml");
            var s = await file.OpenReadAsync();
            var xeFixtureDb = XElement.Load(s.AsStreamForRead());
            s.Dispose();
            var fixtDb = LoaderFixtureDatabase.LoadFromXml(xeFixtureDb);
            return fixtDb;
        }

        private async Task<StorageFile> GetPackagedFile(string folderName, string fileName)
        {
            StorageFolder installFolder = Windows.ApplicationModel.Package.Current.InstalledLocation;

            if (folderName != null)
            {
                StorageFolder subFolder = await installFolder.GetFolderAsync(folderName);
                return await subFolder.GetFileAsync(fileName);
            }
            else
            {
                return await installFolder.GetFileAsync(fileName);
            }
        }

        async Task LoadConsoleSurfaceConfiguration()
        {
            // Charge la configuration UI de la console et initialise 
            var file = await GetPackagedFile("Data", "ConsoleSurfaceDefinition.xml");
            var s = await file.OpenReadAsync();
            var xeSurfaceConfiguration = XElement.Load(s.AsStreamForRead());
            s.Dispose();

            BuildSurfaceUi(xeSurfaceConfiguration);

        }

        async Task LoadSurfaceToFixtureMapping()
        {// Cette méthode charge le mapping des controle UI avec les sortie DMX
            var file = await GetPackagedFile("Data\\DmxBinding", "TestBarbarel.xml");
            var s = await file.OpenReadAsync();
            var xeMappings = XElement.Load(s.AsStreamForRead());
            s.Dispose();

            BuildRoutingMatrixAndBinding(xeMappings);
        }


        #region Surface UI Building

        void BuildSurfaceUi(XElement xeSurfaceConfig)
        {
            int nbRows;
            int nbCols;
            var xeSurfDef = xeSurfaceConfig.Element("Surface");
            nbRows = int.Parse(xeSurfDef.Attribute("rows").Value);
            nbCols = int.Parse(xeSurfDef.Attribute("cols").Value);

            var xeCommonPage = xeSurfDef.Element("CommonPage");
            var newGrid = CreatePage(nbCols, nbRows);
            FillGrid(newGrid, xeCommonPage);
            newGrid.Name = "COMMONPAGE";
            grdConsoleArea.Children.Add(newGrid);

            foreach (XElement xePage in xeSurfDef.Elements("Page"))
            {
                var pageGrid = CreatePage(nbCols, nbRows);
                FillGrid(pageGrid, xePage);
                pageGrid.Visibility = Visibility.Collapsed; // TODO : swicth default to Collapsed
                pageGrid.Name = xePage.Attribute("id").Value;
                grdConsoleArea.Children.Add(pageGrid);
                CreateSelectorForPage(pageGrid);
            }
            FAKE_FOR_SCAN();
            FAKE_FOR_TEST_LYRE();
        }

        void FAKE_FOR_SCAN()
        {
            var pageSelectorStyle = this.FindResource("PageSelectorRadioButtonStyle") as Style;

            #region hard code checkbox for SCAN
            var pageScan = grdConsoleArea.FindName("EZSCAN") as Grid;
            CheckBox b;

            b = new CheckBox();
            b.Foreground = new SolidColorBrush(Colors.WhiteSmoke);
            b.Content = " lock X ";
            b.Tag = "lockX";
            b.IsChecked = true;
            b.Checked += scan_Checked;
            b.Unchecked += scan_Unchecked;
            Grid.SetRow(b, 0);
            Grid.SetColumn(b, 5);
            pageScan.Children.Add(b);

            b = new CheckBox();
            b.Foreground = new SolidColorBrush(Colors.WhiteSmoke);
            b.Content = " lock Y ";
            b.Tag = "lockY";
            b.IsChecked = true;
            b.Checked += scan_Checked;
            b.Unchecked += scan_Unchecked;
            Grid.SetRow(b, 0);
            Grid.SetColumn(b, 7);
            pageScan.Children.Add(b);
            #endregion

            }

        void FAKE_FOR_TEST_LYRE()
        {
            #region hard code checkbox for LYRE
            var pageLyre = grdConsoleArea.FindName("LYRE9") as Grid;

            var  b = new CheckBox();
            b.Foreground = new SolidColorBrush(Colors.WhiteSmoke);
            b.Content = " lock PAN ";
            b.Tag = "lockX";
            b.IsChecked = true;
            b.Checked += lyre_Checked;
            b.Unchecked += lyre_Unchecked;
            Grid.SetRow(b, 6);
            Grid.SetColumn(b, 11);
            pageLyre.Children.Add(b);

            b = new CheckBox();
            b.Foreground = new SolidColorBrush(Colors.WhiteSmoke);
            b.Content = " lock TILT ";
            b.Tag = "lockY";
            b.IsChecked = true;
            b.Checked += lyre_Checked;
            b.Unchecked += lyre_Unchecked;
            Grid.SetRow(b, 6);
            Grid.SetColumn(b, 14);
            pageLyre.Children.Add(b);





            #endregion

        }
        

        private void seq_Checked(object sender, RoutedEventArgs e)
        {
            TimeDmxClockEngine.Current.SetParameterValue((sender as CheckBox).Tag.ToString(), "action", "play");
        }

        private void seq_Unchecked(object sender, RoutedEventArgs e)
        {
            TimeDmxClockEngine.Current.SetParameterValue((sender as CheckBox).Tag.ToString(), "action", "stop");
        }


        void scan_Checked(object sender, RoutedEventArgs e)
        {
            TimeDmxClockEngine.Current.SetParameterValue("scanRandom", (sender as CheckBox).Tag.ToString(), "1");
        }

        void scan_Unchecked(object sender, RoutedEventArgs e)
        {
            TimeDmxClockEngine.Current.SetParameterValue("scanRandom", (sender as CheckBox).Tag.ToString(), "0");
        }



        void lyre_Checked(object sender, RoutedEventArgs e)
        {
            TimeDmxClockEngine.Current.SetParameterValue("scanLyre1", (sender as CheckBox).Tag.ToString(), "1");
            TimeDmxClockEngine.Current.SetParameterValue("scanLyre2", (sender as CheckBox).Tag.ToString(), "1");
        }

        void lyre_Unchecked(object sender, RoutedEventArgs e)
        {
            TimeDmxClockEngine.Current.SetParameterValue("scanLyre1", (sender as CheckBox).Tag.ToString(), "0");
            TimeDmxClockEngine.Current.SetParameterValue("scanLyre2", (sender as CheckBox).Tag.ToString(), "0");
        }

        void CreateSelectorForPage(Grid pageToSelect)
        {
            var pageSelectorStyle = this.FindResource("PageSelectorRadioButtonStyle") as Style;

            var sel = new RadioButton();
            sel.Style = pageSelectorStyle;
            sel.Name = "SEL_" + pageToSelect.Name;
            sel.Tag = pageToSelect.Name;
            sel.FontSize = 14;
            sel.Height = 50;
            sel.Content = pageToSelect.Name;
            sel.GroupName = "grpScene";
            sel.Unchecked += sel_Unchecked;
            sel.Checked += sel_Checked;
            if (stkPageSelector.Children.Count == 0)
                sel.IsChecked = true;
            stkPageSelector.Children.Add(sel);

        }

        void sel_Checked(object sender, RoutedEventArgs e)
        {
            // show the attached page grid            
            var sel = sender as RadioButton;
            var grdPage = grdConsoleArea.FindName(sel.Tag.ToString()) as Grid;
            if (grdPage != null)
                grdPage.Visibility = Visibility.Visible;
        }

        void sel_Unchecked(object sender, RoutedEventArgs e)
        {
            // hide the linked page grid
            var sel = sender as RadioButton;
            var grdPage = grdConsoleArea.FindName(sel.Tag.ToString()) as Grid;
            if (grdPage != null)
                grdPage.Visibility = Visibility.Collapsed;
        }

        Grid CreatePage(int nbCols, int nbRows)
        {
            Grid newGrid = new Grid();
            InitGrid(newGrid, nbCols, nbRows, false);
            return newGrid;
        }

        void FillGrid(Grid uiPage, XElement xePageConfig)
        {
            foreach (var xeControl in xePageConfig.Elements())
            {
                switch (xeControl.Name.LocalName)
                {
                    case "OnOff":
                        var butOnOff = new OnOffButton();
                        SetPosition(uiPage, butOnOff, xeControl);
                        uiPage.Children.Add(butOnOff);
                        break;
                    case "ValueReverser":
                        var butDmxReverse = new BindingReverserButton();
                        SetPosition(uiPage, butDmxReverse, xeControl);
                        uiPage.Children.Add(butDmxReverse);
                        break;
                    case "Push":
                        var butPush = new PushButton();
                        SetPosition(uiPage, butPush, xeControl);
                        uiPage.Children.Add(butPush);
                        break;
                    case "MainLcd":
                        var mainLcd = new LcdDisplay();
                        SetPosition(uiPage, mainLcd, xeControl);
                        uiPage.Children.Add(mainLcd);
                        break;
                    case "VerticalSlider":
                        var sld = new VerticalSlider();
                        SetPosition(uiPage, sld, xeControl);
                        uiPage.Children.Add(sld);
                        break;
                    case "Border":
                        var brd = new Border() { BorderThickness = new Thickness(2), BorderBrush = new SolidColorBrush(Colors.White) };
                        SetPosition(uiPage, brd, xeControl);
                        uiPage.Children.Add(brd);
                        break;
                    case "Joystick":
                        var jstick = new Joystick();
                        SetPosition(uiPage, jstick, xeControl);
                        uiPage.Children.Add(jstick);
                        break;
                    case "ColorSelector":
                        var colselect = new ColorSelector();
                        SetPosition(uiPage, colselect, xeControl);
                        uiPage.Children.Add(colselect);
                        break;
                    case "TouchBpm":
                        var tbpmarea = new TouchBpmArea();
                        SetPosition(uiPage, tbpmarea, xeControl);
                        uiPage.Children.Add(tbpmarea);
                        break;

                }
            }
        }

        void SetPosition(Grid targetGrid, FrameworkElement fe, XElement xeConfigControl)
        {
            try
            {
                int row = int.Parse(xeConfigControl.Attribute("row").Value);
                int col = int.Parse(xeConfigControl.Attribute("col").Value);
                int rowspan = 1;
                int colspan = 1;
                XAttribute xa = xeConfigControl.Attribute("rowspan");
                if (xa != null)
                    rowspan = int.Parse(xa.Value);
                xa = xeConfigControl.Attribute("colspan");
                if (xa != null)
                    colspan = int.Parse(xa.Value);
                fe.Name = xeConfigControl.Attribute("id").Value;
                Grid.SetRow(fe, row);
                Grid.SetColumn(fe, col);
                Grid.SetRowSpan(fe, rowspan);
                Grid.SetColumnSpan(fe, colspan);
            }
            catch (Exception ex)
            {
                throw new Exception("Exception while construction surface controle : " + xeConfigControl.ToString(), ex);
            }
        }





        void InitGrid(Grid grd, int nbCol, int nbRow, bool drawCells = false)
        {
            // Creation de la grille
            for (int n = 0; n < nbRow; n++)
            {
                grd.RowDefinitions.Add(new RowDefinition());
            }
            for (int n = 0; n < nbCol; n++)
            {
                grd.ColumnDefinitions.Add(new ColumnDefinition());
            }


            // creation des carré de visualisation des cellules
            if (drawCells)
            {
                for (int c = 0; c < nbCol; c++)
                {
                    for (int r = 0; r < nbRow; r++)
                    {
                        Border b = new Border()
                        {
                            BorderThickness = new Thickness(1),
                            BorderBrush = new SolidColorBrush(Colors.DarkGray),
                            Background = new SolidColorBrush(Colors.Transparent),
                            IsHitTestVisible = false
                        };
                        Grid.SetColumn(b, c);
                        Grid.SetRow(b, r);
                        grd.Children.Add(b);
                    }
                }
            }
        }
        #endregion

        #region Chargement et applicaiton du binding DMX <-> controle via la FastRoutingMatrix


        private void BuildRoutingMatrixAndBinding(XElement xeMappings)
        {
            var xeFixturesUse = xeMappings.Element("Fixtures");
            var fixtureReferences = GetFixtureReferences(xeFixturesUse);

            List<SurfaceItem> lstSurfaceItems = new List<SurfaceItem>(); // list de tout les surface items
            int pageNum = 0;
            foreach (var xePage in xeMappings.Elements("Page"))
            {
                pageNum++;
                var xeRefPageId = xePage.Attribute("ref-pageId");
                if (xeRefPageId==null)
                {
                    Debug.WriteLine($"XML Binding error : Unable to find ref-pageId attribute for page number {pageNum}");
                    continue;
                }
                try
                {
                    var xeName = xePage.Attribute("name");
                    if (xeName != null)
                        SetPageSelectorName(xeRefPageId.Value, xeName.Value);

                    var page = grdConsoleArea.FindName(xeRefPageId.Value) as Grid;
                    foreach (var xeSurfaceItem in xePage.Elements())
                    {
                        try
                        {


                            switch (xeSurfaceItem.Name.LocalName)
                            {
                                case "Slider":
                                    var ssi = CreateSliderSurfaceItem(fixtureReferences, xeSurfaceItem);
                                    var sldUC = page.FindName(xeSurfaceItem.Attribute("ref-id").Value) as VerticalSlider;
                                    if (sldUC == null)
                                        break;
                                    sldUC.DataContext = ssi;
                                    lstSurfaceItems.Add(ssi);
                                    break;
                                case "OnOff":
                                    var oosi = CreateOnOffSurfaceItem(fixtureReferences, xeSurfaceItem);
                                    var onoffUC = page.FindName(xeSurfaceItem.Attribute("ref-id").Value) as OnOffButton;
                                    if (onoffUC == null)
                                        break;
                                    onoffUC.DataContext = oosi;
                                    break;
                                case "Push":
                                    var psi = CreatePushSurfaceItem(fixtureReferences, xeSurfaceItem);
                                    var pushUC = page.FindName(xeSurfaceItem.Attribute("ref-id").Value) as PushButton;
                                    if (pushUC == null)
                                        break;
                                    pushUC.DataContext = psi;
                                    break;
                                case "Joystick":
                                    SliderSurfaceItem xSI, ySI;
                                    CreateJoystickSurfaceItem(fixtureReferences, xeSurfaceItem, out xSI, out ySI);
                                    var joystickUC = page.FindName(xeSurfaceItem.Attribute("ref-id").Value) as Joystick;
                                    if (joystickUC == null)
                                        break;
                                    joystickUC.XSurfaceItem = xSI;
                                    joystickUC.YSurfaceItem = ySI;
                                    lstSurfaceItems.Add(xSI);
                                    lstSurfaceItems.Add(ySI);
                                    break;
                                case "ColorSelector":
                                    SliderSurfaceItem redSI, greenSI, blueSI;
                                    CreateColorSelectorSurfaceItem(fixtureReferences, xeSurfaceItem, out redSI, out greenSI, out blueSI);
                                    var colorSelectorUC = page.FindName(xeSurfaceItem.Attribute("ref-id").Value) as ColorSelector;
                                    if (colorSelectorUC == null)
                                        break;
                                    colorSelectorUC.RedSurfaceItem = redSI;
                                    colorSelectorUC.GreenSurfaceItem = greenSI;
                                    colorSelectorUC.BlueSurfaceItem = blueSI;
                                    lstSurfaceItems.Add(redSI);
                                    lstSurfaceItems.Add(greenSI);
                                    lstSurfaceItems.Add(blueSI);
                                    break;
                                case "ValueReverser":
                                    ValueReverserSurfaceItem reverserSI;
                                    CreateValueReverserSurfaceItem(fixtureReferences, page, xeSurfaceItem, out reverserSI);
                                    var valueReverserUC = page.FindName(xeSurfaceItem.Attribute("ref-id").Value) as BindingReverserButton;
                                    if (valueReverserUC == null)
                                        return;
                                    valueReverserUC.DataContext = reverserSI;
                                    break;
                            }
                        }
                        catch(Exception ex)
                        {
                            Debug.WriteLine($"Error while parsing SurfaceItem '{xeSurfaceItem.Attribute("ref-id").Value}'");
                        }
                    }
                }
                catch(Exception ex)
                {
                    Debug.WriteLine($"Error while parsing Page '{xeRefPageId.Value}'");
                }
            }


            LighJOpenDmxAgentServiceProxy dmxProxy = new LighJOpenDmxAgentServiceProxy("localhost", 10256);
            dmxProxy.Initialize();
            // on a la liste des surface items --> on peut creer la matrice de routage
            FastRoutingMatrix frm = new FastRoutingMatrix(dmxProxy, lstSurfaceItems);
            FastRoutingMatrix.Current = frm;


        }

        private void CreateValueReverserSurfaceItem(Dictionary<string, FixtureReference> fixtureReferences,  Grid page,
            XElement xeSurfaceItem, out ValueReverserSurfaceItem reverserSurfaceItem)
        {
            List<DmxBinding> reverseTargets = new List<DmxBinding>();

            var xaName = xeSurfaceItem.Attribute("name");
            string name = "";
            if (xaName != null)
                name = xaName.Value;

            var xeTargets = xeSurfaceItem.Element("Targets");
            foreach (var xeTarget in xeSurfaceItem.Elements("Target"))
            {
                try
                {
                    // <Target ref-id="lyre9_pan" ref-fixture="lyre2" inputNum="1"/>
                    string refId = xeTarget.Attribute("ref-id").Value;
                    int hackBindingIndex = int.Parse(xeTarget.Attribute("hackBindingIndex").Value);

                    
                    var uc = page.FindName(refId) as VerticalSlider; // TODO : aad management of other control ex:Joystick !
                    if (uc == null)
                        continue;
                    var ssi = uc.DataContext as SliderSurfaceItem; // on recupere le SliderSurfaceItem attaché au slider
                    if (ssi == null)
                        continue;

                    if (ssi.DmxBindings.Count < hackBindingIndex + 1)
                        continue;

                    var binding = ssi.DmxBindings[hackBindingIndex];
                    reverseTargets.Add(binding);
                }
                catch(Exception ex)
                {
                    throw new Exception($"Invalid '{xeTarget.ToString()}' element in '{xeSurfaceItem.ToString()}'");
                }
            }

            reverserSurfaceItem = new ValueReverserSurfaceItem(name, reverseTargets);

            var xaImg = xeSurfaceItem.Attribute("img");
            if (xaImg != null)
                reverserSurfaceItem.ImageResourceName = xaImg.Value;
        }

        private void CreateJoystickSurfaceItem(Dictionary<string, FixtureReference> references,
            XElement xeSurfaceItem, out SliderSurfaceItem xSurfaceItem, out SliderSurfaceItem ySurfaceItem)
        {
            xSurfaceItem = null;
            ySurfaceItem = null;

            var xaName = xeSurfaceItem.Attribute("name");
            string name = "";
            if (xaName != null)
                name = xaName.Value;

            var xePanOutput = xeSurfaceItem.Element("PanOutput");
            if (xePanOutput == null)
                throw new Exception("missing Output element for " + xeSurfaceItem);
            xSurfaceItem = CreateJoystickAxisSurfaceItem(name + "X", references, xePanOutput);

            var xeTiltOutput = xeSurfaceItem.Element("TiltOutput");
            if (xeTiltOutput == null)
                throw new Exception("missing Output element for " + xeSurfaceItem);
            ySurfaceItem = CreateJoystickAxisSurfaceItem(name + "Y", references, xeTiltOutput);



        }

        private SliderSurfaceItem CreateJoystickAxisSurfaceItem(string name, Dictionary<string, FixtureReference> references, XElement xeAxisOutput)
        {
            var xaRefFixtMinMax = xeAxisOutput.Attribute("ref-minmaxfixture");
            var xaRefRangeMinMax = xeAxisOutput.Attribute("ref-minmaxrange");

            byte min = 0;
            byte max = 255;
            byte defaultValue = 0;
            if (xaRefFixtMinMax != null)
            {
                var fixt = fixturesDb.Fixtures[references[xaRefFixtMinMax.Value].DbFixtureId];
                if (fixt == null)
                    throw new Exception("Invalid fixture reference '" + xaRefFixtMinMax.Value + "' in mapping file");
                var rangeMinMax = fixt.GetInputRangeByNum(xaRefRangeMinMax.Value);
                if (rangeMinMax == null)
                    throw new Exception("Invalid range num reference '" + xaRefRangeMinMax.Value + "' in mapping file");
                min = rangeMinMax.Minimum;
                max = rangeMinMax.Maximum;
            }
            var xaDefaultValue = xeAxisOutput.Attribute("default");
            if (xaDefaultValue != null)
                defaultValue = byte.Parse(xaDefaultValue.Value);
            if (defaultValue < min)
                defaultValue = min;

            List<DmxBinding> dmxChannelTargets = BuildBindingList(references, xeAxisOutput);
            var ssi = new SliderSurfaceItem(name, dmxChannelTargets, min, max, defaultValue);
            return ssi;
        }

        private void CreateColorSelectorSurfaceItem(Dictionary<string, FixtureReference> references,
            XElement xeSurfaceItem, out SliderSurfaceItem redSurfaceItem, out SliderSurfaceItem greenSurfaceItem, out SliderSurfaceItem blueSurfaceItem)
        {
            redSurfaceItem = null;
            greenSurfaceItem = null;
            blueSurfaceItem = null;

            var xaName = xeSurfaceItem.Attribute("name");
            string name = "";
            if (xaName != null)
                name = xaName.Value;

            var xeRedOutput = xeSurfaceItem.Element("RedOutput");
            if (xeRedOutput == null)
                throw new Exception("missing Output element for " + xeSurfaceItem);
            redSurfaceItem = CreateJoystickAxisSurfaceItem(name + "RED", references, xeRedOutput);

            var xeGreenOutput = xeSurfaceItem.Element("GreenOutput");
            if (xeGreenOutput == null)
                throw new Exception("missing Output element for " + xeSurfaceItem);
            greenSurfaceItem = CreateJoystickAxisSurfaceItem(name + "GREEN", references, xeGreenOutput);

            var xeBlueOutput = xeSurfaceItem.Element("BlueOutput");
            if (xeBlueOutput == null)
                throw new Exception("missing Output element for " + xeSurfaceItem);
            blueSurfaceItem = CreateJoystickAxisSurfaceItem(name + "BLUE", references, xeBlueOutput);
        }

        private PushSurfaceItem CreatePushSurfaceItem(Dictionary<string, FixtureReference> references,
            XElement xeSurfaceItem)
        {
            List<DmxBinding> bindingsPress = null;
            List<DmxBinding> bindinsReleased = null;

            var xaName = xeSurfaceItem.Attribute("name");
            string name = "";
            if (xaName != null)
                name = xaName.Value;

            foreach (var xeOutput in xeSurfaceItem.Elements("Output"))
            {
                List<DmxBinding> dmxChannelTargets = BuildBindingList(references, xeOutput);
                if (xeOutput.Attribute("state").Value == "PRESS")
                    bindingsPress = dmxChannelTargets;
                else
                    bindinsReleased = dmxChannelTargets;
            }
            var psi = new PushSurfaceItem(name, bindingsPress, bindinsReleased);

            var xaImg = xeSurfaceItem.Attribute("img");
            if (xaImg != null)
                psi.ImageResourceName = xaImg.Value;

            return psi;
        }

        private OnOffSurfaceItem CreateOnOffSurfaceItem(Dictionary<string, FixtureReference> references,
          XElement xeSurfaceItem)
        {
            List<DmxBinding> bindingsOn = null;
            List<DmxBinding> bindinsOff = null;

            var xaName = xeSurfaceItem.Attribute("name");
            string name = "";
            if (xaName != null)
                name = xaName.Value;

            foreach (var xeOutput in xeSurfaceItem.Elements("Output"))
            {
                List<DmxBinding> dmxChannelTargets = BuildBindingList(references, xeOutput);
                if (xeOutput.Attribute("state").Value == "ON")
                    bindingsOn = dmxChannelTargets;
                else
                    bindinsOff = dmxChannelTargets;
            }
            var psi = new OnOffSurfaceItem(name, bindingsOn, bindinsOff);

            var xaImg = xeSurfaceItem.Attribute("img");
            if (xaImg != null)
                psi.ImageResourceName = xaImg.Value;

            return psi;
        }


        private SliderSurfaceItem CreateSliderSurfaceItem(Dictionary<string, FixtureReference> references, XElement xeSurfaceItem)
        {
            var xeOutput = xeSurfaceItem.Element("Output");
            if (xeOutput == null)
                throw new Exception("missing Output element for " + xeSurfaceItem);
            var xaRefFixtMinMax = xeSurfaceItem.Attribute("ref-minmaxfixture");
            var xaRefRangeMinMax = xeSurfaceItem.Attribute("ref-minmaxrange");
            var xaName = xeSurfaceItem.Attribute("name");
            string name = "";
            if (xaName != null)
                name = xaName.Value;
            byte min = 0;
            byte max = 255;
            byte defaultValue = 0;
            string rangeMinMaxRef = null;
            if (xaRefFixtMinMax != null)
            {
                var fixt = fixturesDb.Fixtures[references[xaRefFixtMinMax.Value].DbFixtureId];
                if (fixt == null)
                    throw new Exception("Invalid fixture reference '" + xaRefFixtMinMax.Value + "' in mapping file");
                var rangeMinMax = fixt.GetInputRangeByNum(xaRefRangeMinMax.Value);
                if (rangeMinMax == null)
                    throw new Exception("Invalid range num reference '" + xaRefRangeMinMax.Value + "' in mapping file");
                min = rangeMinMax.Minimum;
                max = rangeMinMax.Maximum;
                rangeMinMaxRef = rangeMinMax.Num;
            }
            var xaDefaultValue = xeOutput.Attribute("default");
            if (xaDefaultValue != null)
                defaultValue = byte.Parse(xaDefaultValue.Value);
            if (defaultValue < min)
                defaultValue = min;

            List<DmxBinding> dmxChannelTargets = BuildBindingList(references, xeOutput, rangeMinMaxRef);

            var ssi = new SliderSurfaceItem(name, dmxChannelTargets, min, max, defaultValue);

            var xaImg = xeSurfaceItem.Attribute("img");
            if (xaImg != null)
                ssi.ImageResourceName = xaImg.Value;
            return ssi;
        }

        private List<DmxBinding> BuildBindingList(Dictionary<string, FixtureReference> references, XElement xeBindings,string rangeNum=null)
        {
            var dmxChannelTargets = new List<DmxBinding>(); 
            foreach (var xeBinding in xeBindings.Elements("Binding"))
            {
                
                byte min = 0;
                byte max = 255;
                var xaRefFixture = xeBinding.Attribute("ref-fixture");
                var xaInputNum = xeBinding.Attribute("inputNum");
                var xaReverse = xeBinding.Attribute("reverse");
                var xaValue = xeBinding.Attribute("value");

               
                    short inputNum = short.Parse(xaInputNum.Value);
                    short realDmxChannel = (short)(inputNum + references[xaRefFixture.Value].DmxChannel - 1);
                    bool reverse = false;
                    if (xaReverse != null)
                    {
                        if (xaReverse.Value.ToLower() == "true")
                            reverse = true;
                    }

                    var fix = fixturesDb.Fixtures[references[xaRefFixture.Value].DbFixtureId];
                    var rang = fix.GetInputRangeByNum(rangeNum);
                    if (rang != null)
                    {
                        min = rang.Minimum;
                        max = rang.Maximum;
                    }

                    byte value = 0;
                    if (xaValue != null)
                        byte.TryParse(xaValue.Value, out value);

                    dmxChannelTargets.Add(new DmxBinding() { Channel = realDmxChannel, Reverse = reverse, DmxMin = min, DmxMax = max, Value = value });
            }

            foreach (var xeBinding in xeBindings.Elements("Preset"))
            {
                // TODO : add preset loading and insert int dmxChannelTarget
            }

            return dmxChannelTargets;
        }


        /// <summary>
        /// Cette méthode change le nom par défaut du selecteur de page avec celui définit dans le mapping
        /// </summary>
        /// <param name="pageId"></param>
        /// <param name="pageName"></param>
        void SetPageSelectorName(string pageId, string pageName)
        {
            var selector = stkPageSelector.FindName("SEL_" + pageId) as RadioButton;
            if (selector != null)
                selector.Content = pageName;
        }

        Dictionary<string, FixtureReference> GetFixtureReferences(XElement xeFixturesUse)
        {
            var lstRef = new Dictionary<string, FixtureReference>();
            foreach (XElement xe in xeFixturesUse.Elements("Fixture"))
            {
                FixtureReference refFixture = new FixtureReference();
                refFixture.Id = xe.Attribute("id").Value;
                refFixture.Name = xe.Attribute("name").Value;
                refFixture.DbFixtureId = xe.Attribute("ref-dbFixtureId").Value;
                refFixture.DmxChannel = byte.Parse(xe.Attribute("dmxChannel").Value);
                lstRef.Add(refFixture.Id, refFixture);
            }
            return lstRef;
        }




        #endregion

        private void butHamburger_Click(object sender, RoutedEventArgs e)
        {
            svMenu.IsPaneOpen = !svMenu.IsPaneOpen;
        }

        SimpleArtnet.SimpleArtNetEngine artnetEngine = null;
        private void butSenArtnetPoll_Click(object sender, RoutedEventArgs e)
        {
            artnetEngine= new SimpleArtnet.SimpleArtNetEngine(
                (node) => ArtNetDeviceAction(node),
                (buf,offset) => { FastRoutingMatrix.Current.CopyDmxBufferTo(buf, offset); }
                );
            artnetEngine.Start();
        }

        

        async void ArtNetDeviceAction(Device node)
        {
            // TODO : code node list ui management
        }

        private void butStopArtnetPoll_Click(object sender, RoutedEventArgs e)
        {
            if (artnetEngine == null)
                return;
            artnetEngine.Stop();
            artnetEngine = null;
        }

        byte currentDmxValueForTest = 0;
        private void butSendArtnetDmx_Click(object sender, RoutedEventArgs e)
        {
            byte[] dmxBuffer = new byte[512];
            currentDmxValueForTest = (byte)((currentDmxValueForTest + 30) % 0xFF);
            dmxBuffer[109] = currentDmxValueForTest;
            dmxBuffer[110] = currentDmxValueForTest; // pan 
            dmxBuffer[111] = currentDmxValueForTest; // tilt
            Debug.WriteLine($"Value = {currentDmxValueForTest}");
            artnetEngine.SendDmx(dmxBuffer,"192.168.92.200");
        }
    }
}
