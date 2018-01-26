using Android.App;
using Android.Widget;
using Android.OS;
using Plugin.Battery;
using System;
using Android.Content;
using System.Threading;
using System.Threading.Tasks;
using System.ComponentModel;
namespace battery
{
    [Activity(Label = "Battery", MainLauncher = true, Icon = "@drawable/icon")]
    public class MainActivity : Activity
    {
        TextView levelText;
        TextView capacity;
        TextView Timer;
        TextView time;
        Thread hilo;
        Button cancel;
        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);
            
            // Set our view from the "main" layout resource
            SetContentView(Resource.Layout.Main);
            Timer=FindViewById<TextView>(Resource.Id.Timer);
           time= FindViewById<TextView>(Resource.Id.Time);
            time.Text = "";
            Timer.Text = "";
            levelText = FindViewById<TextView>(Resource.Id.actualLevel);
            levelText.Text = CrossBattery.Current.RemainingChargePercent.ToString();
            capacity = FindViewById<TextView>(Resource.Id.capacity);
            capacity.Text = "0";
            ShowMessage();
            Button start= FindViewById<Button>(Resource.Id.start);
            start.Click += Start_Click;
            cancel = FindViewById<Button>(Resource.Id.Cancel);
            cancel.Click += Cancel_Click;
            cancel.Visibility = Android.Views.ViewStates.Invisible;
        }

        private void Cancel_Click(object sender, EventArgs e)
        {
            try { hilo.Abort(); }
            catch { }
            System.Environment.Exit(0);
        }

        private void Start_Click(object sender, EventArgs e)
        {

            cancel.Visibility = Android.Views.ViewStates.Visible;
            hilo = new Thread(Clock);
            hilo.Start();
        }
       
        public void UpdateTimer(string time)
        {
            RunOnUiThread(() =>
            {
                FindViewById<TextView>(Resource.Id.Time).Text = "Tiempo restante: ";
                FindViewById<TextView>(Resource.Id.Timer).Text = time;
            });
        }
        public void Clock()
        {
            int min = 4, seg = 59;
            while (min >= 0 && seg >= 0)
            {
                Thread.Sleep(1000);
                seg = seg - 1;
                if (seg == -1)
                {
                    seg = 59;
                    min = min - 1;
                    if (min == -1)
                    {
                        Calcular();
                    }
                    
                }
               
                UpdateTimer(TimerFormat(min, seg));
          }
        }
        public string TimerFormat(int min, int seg)
        {
            string time;
            string segundos;
            if (seg<10)
            {
                segundos = "0" + Convert.ToString(seg);
            }
            else
            {
                segundos = Convert.ToString(seg);

            }
            time = Convert.ToString(min) + ":" + segundos;
            return time;

        }
        public void Calcular()
        {
            
        }
        public void ShowInstructions()
        {
            AlertDialog alert = new AlertDialog.Builder(this).Create();
            AlertDialog aler = new AlertDialog.Builder(this).Create();
            alert.SetMessage("Esta aplicacion cuenta cada 5 minutos el cambio de cantidad de energia en el dispositivo, es mejor que no lo use en ese tiempo");
            alert.SetButton("Continuar", (a, b) =>
            {
                aler.Show();
            });
           
            aler.SetMessage("La aplicacion ya comenzara a medir la velocidad de carga");
            aler.SetButton("Comenzar", (c, f) => {
                Start();
            });
            aler.SetButton2("Salir", (g, h) => {
                System.Environment.Exit(0);
            });
            
            alert.Show();
        }
        public void Start()
        {

        }
        public void ShowFirstMessageAsync()
        {
            
            EditText cap= new EditText(this);
            AlertDialog alert = new AlertDialog.Builder(this).Create();
            
            alert.SetTitle("Configurando");
            alert.SetMessage("Ingrese la capacidad en mah de su bateria, si ingresa un numero inferior a 100 se cerrara la aplicacion");
            alert.SetView(cap);
            alert.SetButton("Continuar", (a, b) =>
            {
                bool verify;
                try {verify = Convert.ToInt32(cap.Text) > 100 ? true : false; }
                catch { verify = false; }
             
                if (!verify)
                {
                    System.Environment.Exit(0);
                }
                else
                {
                    FindViewById<TextView>(Resource.Id.capacity).Text = cap.Text;
                    ShowInstructions();
                }
                
            });

            alert.Show();
           
        }
        public void ShowMessage()
        {
            String Message;
            bool IsCharging = CrossBattery.Current.Status.ToString() == "Charging" ? true : false;
            AlertDialog alert = new AlertDialog.Builder(this).Create();
            if (!IsCharging)
            {
                Message = "El dispositivo no esta en carga, no se puede iniciar la aplicacion";
                alert.SetTitle("Error");
                alert.SetButton("Salir", (a, b) => { System.Environment.Exit(0); });
            }
            else
            {
                Message = "El dispositivo esta listo";
                alert.SetTitle("Listo");
                alert.SetButton("Comenzar", (a, b) => {
                    ShowFirstMessageAsync();

                });
            }
            alert.SetMessage(Message);
            alert.Show();
        }
        
    }
    
}

