# Blocky Tanks: Core Assault - Prototype

## 1. 遊戲標語 (Logline)
在一個由方塊構成的戰術世界裡，駕駛你的模組化坦克，與隊友協力稱霸戰場。這不是傳統的坦克大戰，這是結合了高速機動、致命近戰與精準狙擊的次世代團隊競技。

## 2. 核心概念 (Core Concept)
《方塊坦克：核心突擊》是一款5v5、第一人稱/第三人稱可切換的團隊戰術射擊遊戲。本原型專注於實現遊戲的核心戰鬥和網絡功能。

## 3. 已實現功能 (Implemented Features)
- **三種獨特的坦克職業**:
  - **狙擊手 (Sniper)**: 高斯磁軌砲, 架設模式。
  - **劍士 (Saber)**: 衝擊巨砲 (近戰), 能量護盾, 動力衝刺。
  - **衝鋒者 (Assault)**: 自動霰彈砲, 噴射跳躍。
- **團隊殲滅遊戲模式**: 經典的團隊死鬥模式，包含計分和重生邏輯。
- **網絡功能**:
  - 基於 `Unity.Netcode` 的服務器授權架構。
  - 支持主機 (Host) 和客戶端 (Client) 連接。
  - 網絡同步玩家移動、動作和遊戲狀態（生命值、分數）。
- **完整的遊戲流程**:
  - **主選單**: 可選擇作為主機開始或作為客戶端加入遊戲。
  - **遊戲大廳**: 玩家在比賽開始前集結的區域，可看到已連接的玩家列表。
  - **遊戲內HUD**: 顯示生命值、分數和技能冷卻/狀態。

## 4. Unity專案設定教程 (Unity Project Setup Guide)

這是一個純腳本原型。要將其轉換為可運行的Unity專案，請遵循以下步驟：

### 步驟 1: 建立新專案
1. 打開 Unity Hub 並建立一個新的 **3D URP (通用渲染管線)** 專案。
2. 將此儲存庫中的 `Assets` 文件夾複製到您的新Unity專案根目錄中，覆蓋現有的 `Assets` 文件夾。

### 步驟 2: 安裝 Netcode for GameObjects
1. 在Unity編輯器中，前往 `Window > Package Manager`。
2. 點擊 `+` 圖標，然後選擇 `Add package by name...`。
3. 輸入 `com.unity.transport` 並點擊 `Add`。
4. 再次點擊 `+` 圖標，選擇 `Add package by name...`。
5. 輸入 `com.unity.netcode.gameobjects` 並點擊 `Add`。

### 步驟 3: 設定場景 (Scenes)
1. 建立三個新場景並將它們保存在 `Assets/Scenes` 文件夾中:
   - `MainMenuScene`
   - `LobbyScene`
   - `GameScene`
2. 前往 `File > Build Settings`，並將這三個場景按順序添加到 `Scenes In Build` 列表中。

### 步驟 4: 建立核心Prefab
1. **建立基礎坦克 Prefab**:
   - 在 `GameScene` 中，建立一個膠囊體 (Capsule) 作為坦克基座，另一個方塊 (Cube) 作為砲塔。
   - 將 `TankController.cs`, `Tank.cs`, `Rigidbody` 和 `NetworkObject` 組件添加到父膠囊體。
   - 將 `NetworkTransform` 組件添加到父膠囊體以同步位置。
   - 將 `TankController` 腳本中的 `Turret Transform` 字段指向您的砲塔方塊。
2. **為每個職業製作變體 (Variants)**:
   - **狙擊手**: 複製基礎坦克。添加 `GaussRailgun.cs` (附加到砲塔) 和 `SiegeMode.cs`。將它們拖到 `TankController` 的相應字段中。
   - **劍士**: 複製基礎坦克。添加 `ImpactBlade.cs`, `EnergyShield.cs`, 和 `PowerLunge.cs`。
   - **衝鋒者**: 複製基礎坦克。添加 `AutoShotgun.cs` 和 `JetJump.cs`。
3. **設定網絡 Prefabs**:
   - 在 `NetworkManager` 的設定中 (您需要將一個帶有 `NetworkManager` 組件的GameObject添加到每個場景中)，找到 `Network Prefabs` 列表。
   - 將您為三個坦克職業創建的 Prefab 添加到此列表中。這是網絡生成所必需的。

### 步驟 5: 設定UI和管理器
1. **MainMenuScene**:
   - 建立一個 Canvas。
   - 添加按鈕來觸發 `MainMenu.cs` 中的 `StartHost()` 和 `JoinClient()` 方法。
   - 建立一個空的 GameObject 並將 `MainMenu.cs` 附加到它上面。
2. **LobbyScene**:
   - 建立一個 Canvas。
   - 建立一個空的 GameObject 並將 `LobbyManager.cs` 附加到它上面。
   - 按照 `LobbyManager.cs` 腳本中的 `[SerializeField]` 字段的指示，連接UI元素（玩家列表、開始按鈕）。
3. **GameScene**:
   - 建立一個 Canvas。
   - 建立一個空的 GameObject 並將 `UIManager.cs` 和 `GameManager.cs` 附加到它上面。
   - 連接 `UIManager` 所需的UI文本和圖像元素。
   - 在場景中放置一些方塊作為重生點，並將它們的 Transform 拖到 `GameManager` 的重生點列表中。

## 5. 如何遊玩 (How to Play)
1. **啟動遊戲**:
   - 建立並運行遊戲。
   - **玩家 1 (主機)**: 點擊 "Host Game" 按鈕。您將進入大廳。
   - **玩家 2 (客戶端)**: 啟動第二個遊戲實例。點擊 "Join Game" 按鈕。您應該會加入玩家1的大廳。
2. **開始比賽**:
   - 在大廳中，主機玩家會看到一個 "Start Game" 按鈕。點擊它會將所有玩家加載到遊戲場景中。
3. **遊戲操控**:
   - **WASD**: 移動坦克
   - **滑鼠**: 瞄準砲塔
   - **左鍵**: 開火/攻擊
   - **Q / E**: 使用技能 1 / 技能 2
   - **目標**: 在 "團隊殲滅" 模式中，通過消滅敵方隊伍來得分。首先達到分數上限的隊伍獲勝。
