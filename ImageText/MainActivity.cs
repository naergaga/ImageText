using Android.App;
using Android.OS;
using Android.Support.V7.App;
using Android.Runtime;
using Android.Widget;
using Android.Views;
using System.Threading.Tasks;
using System.IO;
using Android.Content;
using Android.Net;
using Xamarin.Essentials;
using static Android.Widget.TextView;
using Tesseract.Droid;

namespace ImageText
{
    [Activity(Label = "@string/app_name", Theme = "@style/AppTheme", MainLauncher = true)]
    public class MainActivity : AppCompatActivity
    {
        public const int PickImageId = 1000;

        ImageView imageView = null;
        TextView textView = null;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            Xamarin.Essentials.Platform.Init(this, savedInstanceState);
            // Set our view from the "main" layout resource
            SetContentView(Resource.Layout.activity_main);
            imageView = FindViewById<ImageView>(Resource.Id.imageView1);
            textView = FindViewById<TextView>(Resource.Id.textView1);
            Permissions.RequestAsync<Permissions.StorageRead>();
        }

        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Android.Content.PM.Permission[] grantResults)
        {
            Xamarin.Essentials.Platform.OnRequestPermissionsResult(requestCode, permissions, grantResults);

            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
        }

        protected override async void OnActivityResult(int requestCode, [GeneratedEnum] Result resultCode, Intent data)
        {
            base.OnActivityResult(requestCode, resultCode, data);
            if (requestCode == PickImageId)
            {
                if (resultCode == Result.Ok && Intent != null)
                {
                    Uri uri = data.Data;
                    imageView.SetImageURI(uri);
                    Stream stream = ContentResolver.OpenInputStream(uri);
                    TesseractApi api = new TesseractApi(this, AssetsDeployment.OncePerVersion);
                    await api.Init("chi_sim");
                    await api.SetImage(PathUtil.GetActualPathFromFile(this, data.Data));
                    string text = api.Text;
                    textView.SetText(text, BufferType.Normal);
                }
            }
        }

        [Java.Interop.Export("BtnOpenImageClick")]
        public void BtnOpenImageClick(View v)
        {
            var intent = new Intent(Intent.ActionPick);
            intent.SetType("image/*");
            StartActivityForResult(intent, PickImageId);
        }
    }
}