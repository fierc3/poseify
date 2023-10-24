using Core.Services.Estimations;
using Core.Services.Queues;
using Raven.Client.Documents;
using System.Diagnostics;
using System.IO;

namespace Core.Services.Fbx
{
    public class FbxService : IFbxService
    {
        private readonly ILogger<FbxService> _logger;
        private readonly IConfiguration _configuration;

        public FbxService(ILogger<FbxService> logger, IConfiguration configuration)
        {
            _logger = logger;
            _configuration = configuration;
        }

        public void CreateFbxFileFromBvh(string bvhLocation)
        {
            string? fbxScriptLocation = _configuration["FbxScriptLocation"];
            string? fbxBlender = _configuration["FbxBlender"];

            // Goal Process:
            // C:\Program Files\Blender Foundation\Blender 3.2> .\blender.exe --background --python D:\hslul\poseify\src\Core\FbxGeneration\bvh2fbx.py --bvhFileLocation D:/hslul/VideoTo3dPoseAndBvh/bhvoutput/bvh/motioncapture.bvh

            string lastError = "";
            Process fbxGenerationProcess = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = string.IsNullOrEmpty(fbxBlender) ? "blender.exe" : fbxBlender,
                    Arguments = $"--background --python {fbxScriptLocation} --bvhFileLocation {bvhLocation}",
                    UseShellExecute = false,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    CreateNoWindow = true
                },
                EnableRaisingEvents = true,
            };
            fbxGenerationProcess.OutputDataReceived += (se, ev) =>
            {
                _logger.Log(LogLevel.Information, ev.Data);
            };
            fbxGenerationProcess.ErrorDataReceived += (se, ev) =>
            {
                _logger.Log(LogLevel.Error, ev.Data);
                lastError = ev.Data ?? String.Empty;
            };

            try
            {
                fbxGenerationProcess.Start();
                fbxGenerationProcess.BeginOutputReadLine();
                fbxGenerationProcess.BeginErrorReadLine();
                fbxGenerationProcess.WaitForExit();

                if (!File.Exists(bvhLocation+".fbx"))
                {
                    throw new Exception("fbx file could not be created: " + lastError);
                }

            }
            catch (Exception ex)
            {
                _logger.LogError("FBX Generation failed", ex);
                throw;
            }
        }
    }
}
