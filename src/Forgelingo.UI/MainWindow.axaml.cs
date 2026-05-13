using Avalonia.Controls;
using System;
using System.IO;
using System.Threading.Tasks;
using Forgelingo.Core;
using Forgelingo.Core.AI;

namespace Forgelingo.UI
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            var btn = this.FindControl<Button>("BtnSelectFolder");
            btn.Click += async (_, __) => await OnSelectFolder();
        }

        private async Task OnSelectFolder()
        {
            var dlg = new OpenFolderDialog { Title = "Select Mods Folder" };
            var path = await dlg.ShowAsync(this);
            var status = this.FindControl<TextBlock>("Status");
            if (string.IsNullOrEmpty(path)) { status.Text = "No folder"; return; }
            status.Text = "Processing...";

            // simple orchestrator run
            var ai = new AIEngineSkeleton(apiKey: "");
            var orch = new ForgelingoOrchestrator(ai);
            // run in background to avoid UI block
            await Task.Run(async () =>
            {
                var jars = Directory.GetFiles(path, "*.jar");
                var outdir = Path.Combine(path, "_ResourcePack");
                Directory.CreateDirectory(outdir);
                foreach (var jar in jars) await orch.ProcessJarAsync(jar, outdir);
            });

            status.Text = "Done";
        }
    }
}
