# NHK2025B Controller
## How to Use Controller
### Platforms
OS:
```
- Ubuntu 22.04 (bash)
```

ROS2 distribution:
```
- Humble
```

Unity Editor Version: 
```
- 2022.3.16f1
```

Required Editor Modules:
```
- Android Build Support
    - OpenJDK
    - Android SDK & NDK Tools
- 日本語
```
### Build This Project
- Clone this project.
```bash
git clone git@github.com:NITIC-Robot-Club/KRB2025B_controller.git
```
- Create unity project from cloned disk.
- Open ```File``` > ```Buid Settings```.
- Switch platform to ```Android```.
- Check a ```Development Build``` box.
### Step0 Environmental Preparation(optionally)
- Install softwares
```bash
# Install rmw and tests-msgs for your ROS2 distribution
sudo apt-get update
sudo apt install -y ros-humble-test-msgs
sudo apt install -y ros-humble-fastrtps ros-humble-rmw-fastrtps-cpp
sudo apt install -y ros-humble-cyclonedds ros-humble-rmw-cyclonedds-cpp

# Install vcstool package
curl -s https://packagecloud.io/install/repositories/dirk-thomas/vcstool/script.deb.sh | sudo bash
sudo apt-get update
sudo apt-get install -y python3-vcstool

# Install dotnet
sudo apt-get install -y apt-transport-https

sudo apt remove dotnet* aspnetcore* netstandard*
sudo rm /etc/apt/sources.list.d/microsoft-prod.list
sudo apt update
sudo apt install -y dotnet-sdk-6.0
```
- It is convenient to include the following command in ```~/.profile```.
```bash
. /opt/ros/humble/setup.bash
```

### Step1 Make a Project Folders
- The ``Ros2 For Unity`` provided by ```RobotecAI``` only supports Linux or Windows, so you will need to use a project created by a volunteer to use it on Android.
```bash
cd
mkdir ros2-for-unity_ws
cd ~/ros2-for-unity_ws
mkdir linux_windows_build
mkdir android_build
```
### Step2 Clone Ros2 For Unity Projects
#### Build for Linux or Windows
- Clone [this project](https://github.com/RobotecAI/ros2-for-unity)
```bash
cd ~/ros2-for-unity_ws/linux_windows_build
git clone -b humble git@github.com:RobotecAI/ros2-for-unity.git
cd ~/linux_windows_build/ros2-for-unity
./pull_repositories.sh
```
- Build```Ros2 For Unity```.
```bash
# standalone mode
./build.sh --standalone

# overlay mode
./build.sh

# recommend
./build.sh --clean-install --standalone
```
- You can add ```--clean-install```.
- Unity Asset is ready to import into your Unity project. You can find it in ```install/asset/``` directory.
#### Build for Android
- Clone [this project](https://github.com/hiro-han/ros2-for-unity)
```bash
cd ~/ros2-for-unity_ws/android_build
git clone git@github.com:hiro-han/ros2-for-unity.git
cd ros2-for-unity
./pull_repositories.sh
```
- Build```Ros2 For Unity```.
```bash
# standalone mode
./build.sh -p ~/Unity/Hub/Editor/2022.3.16f1/Editor/Data/PlaybackEngines/AndroidPlayer/NDK  --standalone

# overlay mode
./build.sh -p ~/Unity/Hub/Editor/2022.3.16f1/Editor/Data/PlaybackEngines/AndroidPlayer/NDK

# recommend
./build.sh -p ~/Unity/Hub/Editor/2022.3.16f1/Editor/Data/PlaybackEngines/AndroidPlayer/NDK --clean-install --standalone
```
- You can add ```--clean-install```.
- Unity Asset is ready to import into your Unity project. You can find it in ```install/asset/``` directory.

## How to Build Custom Message for Android
