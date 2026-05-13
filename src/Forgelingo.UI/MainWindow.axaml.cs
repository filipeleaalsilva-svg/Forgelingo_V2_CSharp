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
            var settings = this.FindControl<Button>("BtnSettings");
            settings.Click += async (_, __) => await OnSettings();
        }

        private async Task OnSettings()
        {
            var dlg = new SettingsWindow();
            await dlg.ShowDialog(this);
        }

        private async Task OnSelectFolder()
        {
            var dlg = new OpenFolderDialog { Title = "Select Mods Folder" };
            var path = await dlg.ShowAsync(this);
            var log = this.FindControl<TextBox>("LogBox");
            var prog = this.FindControl<ProgressBar>("Progress");
            if (string.IsNullOrEmpty(path)) { log.Text += "No folder selected\n"; return; }
            log.Text += $"Processing folder: {path}\n";
            prog.Value = 0;

            var apiKey = SettingsManager.LoadApiKey();
            IAIEngine ai;
            if (!string.IsNullOrEmpty(apiKey)) ai = new DeepSeekAIEngine(apiKey);
            else ai = new AIEngineSkeleton("");

            var mem = new TranslationMemory();
            var orch = new ForgelingoOrchestrator(ai, mem);

            await Task.Run(async () =>
            {
                var jars = Directory.GetFiles(path, "*.jar");
                var outdir = Path.Combine(path, "_ResourcePack");
                Directory.CreateDirectory(outdir);
                int i = 0;
                foreach (var jar in jars)
                {
                    log.Text += $"Processing {Path.GetFileName(jar)}\n";
                    await orch.ProcessJarAsync(jar, outdir);
                    i++;
                    prog.Value = (double)i / jars.Length * 100.0;
                }
            });

            log.Text += "Done\n";
            prog.Value = 100;
        }
    }
}
