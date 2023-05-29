
################################################
# VidePose3d and requirements install script   #
################################################

[Net.ServicePointManager]::SecurityProtocol = [Net.SecurityProtocolType]::Tls12

$git_url = "https://github.com/git-for-windows/git/releases/download/v2.39.2.windows.1/Git-2.39.2-64-bit.exe"
$vp3d_url = "https://github.com/MadMeister/VideoPose3D_poseify.git"
$msbuild_url = "https://aka.ms/vs/17/release/vs_buildtools.exe"
$detectron_url = "git+https://github.com/fierc3/detectron2-python-37"
$pretrained_model_url = "https://dl.fbaipublicfiles.com/video-pose-3d/pretrained_h36m_detectron_coco.bin"

$git_dest = "C:\Program Files\Git"
$vp3d_dest = Split-Path -parent $PSCommandPath
$vp3d_dest = $vp3d_dest+"\vp3d"
$msbuild_dest = "C:\Program Files (x86)\Microsoft Visual Studio\2022\BuildTools\MSBuild\Current\Bin"
$pretrained_model_dest = "$vp3d_dest\checkpoint\pretrained_h36m_detectron_coco.bin"


"This Script will install the following things:
    - git in $git_dest
    - videopose3d in $vp3d_dest
    - msbuildtools c++ in $msbuild_dest
    - detectron and various python libraries

if you would like to change any paths please edit the paths in this powershell script"


$git_conf = Read-Host "do you want to install git to $git_dest ? (y/n)"

if ($git_conf -eq 'y') {
    if (-Not(Test-Path $git_dest)) {
        try {
            "installing git"
            Invoke-WebRequest -Uri $git_url -OutFile $git_dest
            $gitExePath = "$git_dest\git.exe"
            Start-Process $gitExePath -Wait -ArgumentList '/NORESTART /NOCANCEL /SP- /CLOSEAPPLICATIONS /RESTARTAPPLICATIONS /COMPONENTS="icons,ext\reg\shellhere,assoc,assoc_sh" /LOG="C:\git-for-windows.log"'
        }
        catch {
            Write-Host "error while downloading and installing git"
            Write-Host $_
        }
        else {
            "git path already exists, skipping installation"
        }
    }
}

$msbuild_conf = Read-Host "do you want to install msbuildtools to $msbuild_dest ? (y/n)"

if ($msbuild_conf -eq 'y') {
    if (-Not(Test-Path $msbuild_dest)) {
        try {
            "installing msbuildtools"
            $msbuildExePath = "$msbuild_dest\vs_buildtools.exe"
            Invoke-WebRequest -Uri $msbuild_url -OutFile  $msbuild_dest 

            $buildtoolargs = " --quiet --norestart --wait --add Microsoft.VisualStudio.Workload.MSBuildTools --add Microsoft.VisualStudio.Component.VC.Tools.x86.x64 --add Microsoft.VisualStudio.Component.VC.CMake.Project --add Microsoft.VisualStudio.Component.VC.ASAN"
            Start-Process $msbuildExePath -ArgumentList $buildtoolargs -Wait -PassThru -WorkingDirectory $msbuild_dest -NoNewWindow
        }
        catch {
            Write-Host "error while downloading and installing msbuildtools"
            Write-Host $_
        }
    }
    else {
        "msbuildtools path already exists, skipping installation"
    }
}

$vp3d_conf = Read-Host "do you want to install videopose3d to $vp3d_dest ? (y/n)"

if ($vp3d_conf -eq 'y') {
    if (-Not(Test-Path $vp3d_dest)) {
        try {
            "installing vp3d"
            git clone $vp3d_url $vp3d_dest
        }
        catch {
            Write-Host "error while downloading and installing vp3d"
            Write-Host $_
        }
    }
    else {
        "vp3d path already exists, skipping installation"
    }
 $CheckPointFolder = "$vp3d_dest\checkpoint"
 if (Test-Path -Path $CheckPointFolder) {
     "Checkpoint folder exists, no reason to create"
 } else {
     "Checkpoint folder is missing, creating it."
     mkdir "$vp3d_dest\checkpoint"
 }

}

$model_conf = Read-Host "do you want to download the pretrained model for vp3d? (y/n)"

if ($model_conf -eq 'y') {
    try {
        "downloading pretrained vp3d model"
        Invoke-WebRequest -Uri $pretrained_model_url -OutFile $pretrained_model_dest 
    }
    catch {
        Write-Host "error while downloading and installing vp3d model"
        Write-Host $_
    }
}

$req_conf = Read-Host "do you want to install detectron2 and all python requirements for videopose3d? as specified in .\requirements.txt ? (y/n)"

if ($req_conf -eq 'y') {
    try {
        "installing requirements"
        python -m pip install $detectron_url
        python -m pip install -r .\requirements.txt
    }
    catch {
        Write-Host "error while downloading and installing detectron2 and or requirements"
        Write-Host $_
    }
}

$ffmpeg_conf = Read-Host "do you want to install ffmpeg for windows (c:\ffmpeg)? (y/n)"

if ($ffmpeg_conf -eq 'y') {
    try {
        "installing ffmpeg"
        New-Item -Type Directory -Path C:\ffmpeg ; Set-Location C:\ffmpeg
        curl.exe -L 'https://www.gyan.dev/ffmpeg/builds/ffmpeg-release-essentials.zip' -o 'ffmpeg.zip'
        Expand-Archive .\ffmpeg.zip -Force -Verbose
        # Move the executable (*.exe) files to the top folder
        Get-ChildItem -Recurse `
            -Path .\ffmpeg\ -Filter *.exe |
        ForEach-Object {
            Write-Output $_
            Move-Item $_.FullName -Destination C:\ffmpeg -Verbose
        }
        Remove-Item .\ffmpeg.zip
        # List the directory contents
        Get-ChildItem
        # Prepend the FFmpeg folder path to the system path variable
        [System.Environment]::SetEnvironmentVariable(
            "PATH",
            "C:\ffmpeg\;$([System.Environment]::GetEnvironmentVariable('PATH','MACHINE'))",
            "Machine"
        )
    }
    catch {
        Write-Host "error while downloading and installing ffmpeg on windows"
        Write-Host $_
    }
}

"IM WELL AND TRULY FINISHED"
Read-Host