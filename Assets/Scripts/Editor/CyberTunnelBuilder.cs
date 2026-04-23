using UnityEngine;
using UnityEditor;
using UnityEngine.UI;
using UnityEngine.Rendering.Universal;
using TMPro;
using System.Collections.Generic;
using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;

public class CyberTunnelBuilder : EditorWindow
{
    [MenuItem("CyberTunnel/Build Entire Game %#g")]
    public static void BuildAll()
    {
        if (!EditorUtility.DisplayDialog("Build CyberTunnel",
            "This will create GameScene and MainMenu scenes.\nExisting scenes with these names will be overwritten.\n\nContinue?",
            "Build", "Cancel"))
            return;

        CreateMaterials();
        BuildGameScene();
        BuildMainMenuScene();
        SetupBuildSettings();

        EditorUtility.DisplayDialog("Done", "CyberTunnel game built successfully!\n\n" +
            "Scenes created:\n- Assets/Scenes/GameScene.unity\n- Assets/Scenes/MainMenu.unity\n\n" +
            "Hit Play in GameScene to test!", "Nice!");
    }

    [MenuItem("CyberTunnel/Build Game Scene Only")]
    public static void BuildGameSceneOnly()
    {
        CreateMaterials();
        BuildGameScene();
    }

    [MenuItem("CyberTunnel/Build Main Menu Only")]
    public static void BuildMainMenuOnly()
    {
        CreateMaterials();
        BuildMainMenuScene();
    }

    // ─────────────────────────────────────────────
    //  MATERIALS
    // ─────────────────────────────────────────────

    static Material matFloor, matWall, matCeiling, matDoor, matDoorFrame;
    static Material matTerminalBody, matTerminalScreen, matTerminalScreenOff;
    static Material matNeonCyan, matNeonGreen, matNeonMagenta, matNeonRed, matNeonBlue;
    static Material matTrim, matLava;
    static Material matDiscoTile, matDiscoBall;

    static void CreateMaterials()
    {
        string folder = "Assets/Materials";
        if (!AssetDatabase.IsValidFolder(folder))
            AssetDatabase.CreateFolder("Assets", "Materials");

        Shader urpLit = Shader.Find("Universal Render Pipeline/Lit");
        Shader urpUnlit = Shader.Find("Universal Render Pipeline/Unlit");

        matFloor = GetOrCreateMaterial(folder, "M_Floor", urpLit,
            new Color(0.05f, 0.05f, 0.08f), 0.8f, 0.1f);

        matWall = GetOrCreateMaterial(folder, "M_Wall", urpLit,
            new Color(0.06f, 0.06f, 0.1f), 0.6f, 0.2f);

        matCeiling = GetOrCreateMaterial(folder, "M_Ceiling", urpLit,
            new Color(0.03f, 0.03f, 0.05f), 0.9f, 0.05f);

        matDoor = GetOrCreateMaterial(folder, "M_Door", urpLit,
            new Color(0.15f, 0.15f, 0.2f), 0.3f, 0.6f);

        matDoorFrame = GetOrCreateMaterial(folder, "M_DoorFrame", urpLit,
            new Color(0.08f, 0.08f, 0.12f), 0.5f, 0.3f);

        matTerminalBody = GetOrCreateMaterial(folder, "M_TerminalBody", urpLit,
            new Color(0.1f, 0.1f, 0.12f), 0.4f, 0.5f);

        matTerminalScreen = GetOrCreateEmissiveMaterial(folder, "M_TerminalScreen", urpLit,
            new Color(0.0f, 0.05f, 0.02f), new Color(0f, 0.8f, 0.3f), 2f);

        matTerminalScreenOff = GetOrCreateMaterial(folder, "M_TerminalScreenOff", urpLit,
            new Color(0.02f, 0.02f, 0.03f), 0.9f, 0.05f);

        matNeonCyan = GetOrCreateEmissiveMaterial(folder, "M_NeonCyan", urpLit,
            new Color(0f, 0.1f, 0.12f), new Color(0f, 0.9f, 1f), 5f);

        matNeonGreen = GetOrCreateEmissiveMaterial(folder, "M_NeonGreen", urpLit,
            new Color(0.02f, 0.1f, 0f), new Color(0.1f, 1f, 0.2f), 5f);

        matNeonMagenta = GetOrCreateEmissiveMaterial(folder, "M_NeonMagenta", urpLit,
            new Color(0.1f, 0f, 0.08f), new Color(1f, 0.1f, 0.8f), 5f);

        matNeonRed = GetOrCreateEmissiveMaterial(folder, "M_NeonRed", urpLit,
            new Color(0.1f, 0f, 0f), new Color(1f, 0.15f, 0.1f), 4f);

        matNeonBlue = GetOrCreateEmissiveMaterial(folder, "M_NeonBlue", urpLit,
            new Color(0f, 0.02f, 0.12f), new Color(0.2f, 0.4f, 1f), 5f);

        matTrim = GetOrCreateEmissiveMaterial(folder, "M_Trim", urpLit,
            new Color(0.01f, 0.01f, 0.02f), new Color(0f, 0.5f, 0.7f), 1.5f);

        matLava = GetOrCreateEmissiveMaterial(folder, "M_Lava", urpLit,
            new Color(0.15f, 0.02f, 0f), new Color(1f, 0.3f, 0f), 3f);

        matDiscoTile = GetOrCreateEmissiveMaterial(folder, "M_DiscoTile", urpLit,
            new Color(0.05f, 0.05f, 0.07f), new Color(0.08f, 0.08f, 0.12f), 0.4f);

        matDiscoBall = GetOrCreateMaterial(folder, "M_DiscoBall", urpLit,
            new Color(0.88f, 0.88f, 0.92f), 0.95f, 0.92f);

        AssetDatabase.SaveAssets();
    }

    static Material GetOrCreateMaterial(string folder, string name, Shader shader,
        Color baseColor, float smoothness, float metallic)
    {
        string path = $"{folder}/{name}.mat";
        Material mat = AssetDatabase.LoadAssetAtPath<Material>(path);
        if (mat != null) return mat;

        mat = new Material(shader);
        mat.name = name;
        mat.SetColor("_BaseColor", baseColor);
        mat.SetFloat("_Smoothness", smoothness);
        mat.SetFloat("_Metallic", metallic);
        AssetDatabase.CreateAsset(mat, path);
        return mat;
    }

    static Material GetOrCreateEmissiveMaterial(string folder, string name, Shader shader,
        Color baseColor, Color emissiveColor, float emissiveIntensity)
    {
        string path = $"{folder}/{name}.mat";
        Material mat = AssetDatabase.LoadAssetAtPath<Material>(path);
        if (mat != null) return mat;

        mat = new Material(shader);
        mat.name = name;
        mat.SetColor("_BaseColor", baseColor);
        mat.SetFloat("_Smoothness", 0.9f);
        mat.EnableKeyword("_EMISSION");
        mat.SetColor("_EmissionColor", emissiveColor * emissiveIntensity);
        mat.globalIlluminationFlags = MaterialGlobalIlluminationFlags.RealtimeEmissive;
        AssetDatabase.CreateAsset(mat, path);
        return mat;
    }

    // ─────────────────────────────────────────────
    //  GAME SCENE
    // ─────────────────────────────────────────────

    static readonly float ROOM_WIDTH = 10f;
    static readonly float ROOM_HEIGHT = 4f;
    static readonly float ROOM_DEPTH = 10f;
    static readonly float CORRIDOR_WIDTH = 3f;
    static readonly float CORRIDOR_LENGTH = 5f;
    static readonly float WALL_THICKNESS = 0.3f;

    static readonly string[] ROOM_NAMES = {
        "Caesar Cipher", "Vigenere Cipher", "Binary Decode",
        "IP Binary Conversion", "Cyber Definitions Quiz"
    };

    static readonly Color[] ROOM_ACCENT_COLORS = {
        new Color(0f, 1f, 0.6f),
        new Color(0f, 0.7f, 1f),
        new Color(1f, 0.3f, 0.8f),
        new Color(1f, 0.6f, 0f),
        new Color(0.3f, 0.5f, 1f)
    };

    static void BuildGameScene()
    {
        var scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
        scene.name = "GameScene";

        // -- Render settings --
        RenderSettings.ambientMode = UnityEngine.Rendering.AmbientMode.Flat;
        RenderSettings.ambientLight = new Color(0.02f, 0.02f, 0.04f);
        RenderSettings.fog = true;
        RenderSettings.fogMode = FogMode.Exponential;
        RenderSettings.fogColor = new Color(0.01f, 0.01f, 0.03f);
        RenderSettings.fogDensity = 0.02f;

        // -- Root containers --
        GameObject envRoot = new GameObject("--- ENVIRONMENT ---");
        GameObject roomsRoot = new GameObject("--- ROOMS ---");
        GameObject uiRoot = new GameObject("--- UI ---");
        GameObject systemsRoot = new GameObject("--- SYSTEMS ---");

        // -- Lighting --
        CreateDirectionalLight();

        // -- Player --
        GameObject player = CreatePlayer();
        player.transform.position = new Vector3(0, 1f, -12f);

        // -- Entry area (outside + corridor into tunnel) --
        BuildEntryArea(envRoot.transform);

        // -- Entry door (starts open, closes when player enters) --
        GameObject entryDoorObj = BuildDoor(-1, new Vector3(0, 0, -ROOM_DEPTH / 2f));
        entryDoorObj.name = "Door_Entry";
        entryDoorObj.transform.SetParent(envRoot.transform);
        Door entryDoorScript = entryDoorObj.GetComponentInChildren<Door>();
        SerializedObject entryDoorSo = new SerializedObject(entryDoorScript);
        entryDoorSo.FindProperty("startOpen").boolValue = true;
        entryDoorSo.ApplyModifiedPropertiesWithoutUndo();

        // -- Build rooms and corridors --
        List<GameObject> roomObjects = new List<GameObject>();
        List<GameObject> doorObjects = new List<GameObject>();
        List<GameObject> terminalObjects = new List<GameObject>();

        for (int i = 0; i < 5; i++)
        {
            float roomZ = i * (ROOM_DEPTH + CORRIDOR_LENGTH);
            Vector3 roomCenter = new Vector3(0, ROOM_HEIGHT / 2f, roomZ);

            GameObject room = BuildRoom(i, roomCenter, envRoot.transform);
            roomObjects.Add(room);

            GameObject terminal = BuildTerminal(i, roomCenter);
            terminal.transform.SetParent(room.transform);
            terminalObjects.Add(terminal);

            if (i == 3)
            {
                IPBinaryPuzzle ipP = terminal.GetComponent<IPBinaryPuzzle>();
                if (ipP != null)
                    BuildIPBinaryRoomWallText(room.transform, ipP);
            }

            if (i < 4)
            {
                float corridorZ = roomZ + ROOM_DEPTH / 2f + CORRIDOR_LENGTH / 2f;
                BuildCorridor(i, new Vector3(0, ROOM_HEIGHT / 2f, corridorZ), envRoot.transform);

                GameObject door = BuildDoor(i, new Vector3(0, 0, roomZ + ROOM_DEPTH / 2f));
                door.transform.SetParent(envRoot.transform);
                doorObjects.Add(door);
            }
        }

        // -- Final exit door --
        float lastRoomZ = 4 * (ROOM_DEPTH + CORRIDOR_LENGTH);
        GameObject exitDoor = BuildDoor(4, new Vector3(0, 0, lastRoomZ + ROOM_DEPTH / 2f));
        exitDoor.name = "Door_Exit";
        exitDoor.transform.SetParent(envRoot.transform);
        doorObjects.Add(exitDoor);

        // -- Exit room (victory area) --
        float exitZ = lastRoomZ + ROOM_DEPTH / 2f + CORRIDOR_LENGTH / 2f;
        BuildExitArea(new Vector3(0, ROOM_HEIGHT / 2f, exitZ + CORRIDOR_LENGTH), envRoot.transform);

        // -- Wire up GameManager + Room components --
        GameObject gmObj = new GameObject("GameManager");
        gmObj.transform.SetParent(systemsRoot.transform);
        GameManager gm = gmObj.AddComponent<GameManager>();

        List<Room> roomScripts = new List<Room>();

        for (int i = 0; i < 5; i++)
        {
            Room roomScript = roomObjects[i].GetComponent<Room>();
            if (roomScript == null)
                roomScript = roomObjects[i].AddComponent<Room>();

            SerializedObject so = new SerializedObject(roomScript);
            so.FindProperty("roomIndex").intValue = i;
            so.FindProperty("roomName").stringValue = ROOM_NAMES[i];

            PuzzleBase puzzle = roomObjects[i].GetComponentInChildren<PuzzleBase>();
            if (puzzle != null)
                so.FindProperty("puzzle").objectReferenceValue = puzzle;

            PuzzleTerminal terminal = terminalObjects[i].GetComponent<PuzzleTerminal>();
            if (terminal != null)
                so.FindProperty("terminal").objectReferenceValue = terminal;

            if (i < doorObjects.Count)
            {
                Door doorScript = doorObjects[i].GetComponentInChildren<Door>();
                if (doorScript != null)
                    so.FindProperty("exitDoor").objectReferenceValue = doorScript;
            }

            Light roomLight = roomObjects[i].GetComponentInChildren<Light>();
            if (roomLight != null)
                so.FindProperty("roomLight").objectReferenceValue = roomLight;

            so.ApplyModifiedPropertiesWithoutUndo();
            roomScripts.Add(roomScript);
        }

        SerializedObject gmSo = new SerializedObject(gm);
        SerializedProperty roomsList = gmSo.FindProperty("rooms");
        roomsList.arraySize = roomScripts.Count;
        for (int i = 0; i < roomScripts.Count; i++)
            roomsList.GetArrayElementAtIndex(i).objectReferenceValue = roomScripts[i];
        gmSo.ApplyModifiedPropertiesWithoutUndo();

        // -- Audio Manager --
        GameObject audioObj = new GameObject("AudioManager");
        audioObj.transform.SetParent(systemsRoot.transform);
        audioObj.AddComponent<AudioManager>();
        AudioSource musicSrc = audioObj.AddComponent<AudioSource>();
        musicSrc.loop = true;
        musicSrc.playOnAwake = true;
        musicSrc.volume = 0.3f;
        AudioSource sfxSrc = audioObj.AddComponent<AudioSource>();

        SerializedObject amSo = new SerializedObject(audioObj.GetComponent<AudioManager>());
        amSo.FindProperty("musicSource").objectReferenceValue = musicSrc;
        amSo.FindProperty("sfxSource").objectReferenceValue = sfxSrc;
        amSo.ApplyModifiedPropertiesWithoutUndo();

        // -- Scene Loader --
        GameObject slObj = new GameObject("SceneLoader");
        slObj.transform.SetParent(systemsRoot.transform);
        slObj.AddComponent<SceneLoader>();

        // -- UI --
        BuildGameUI(uiRoot.transform, roomScripts);

        // -- Alarm overlay + entry trigger --
        {
            Canvas hudCanvas = null;
            foreach (Canvas c in Object.FindObjectsByType<Canvas>(FindObjectsSortMode.None))
            {
                if (c.gameObject.name == "HUD_Canvas") { hudCanvas = c; break; }
            }

            if (hudCanvas != null)
            {
                GameObject alarmBg = CreatePanel("AlarmPanel", hudCanvas.transform,
                    new Color(0.1f, 0f, 0f, 0.7f));
                alarmBg.SetActive(false);

                CreateUIText("AlarmText", alarmBg.transform,
                    Vector2.zero, new Vector2(900, 120),
                    "ALERT: SOLVE 5 PUZZLES TO ESCAPE", 52,
                    new Color(1f, 0.15f, 0.1f), TextAlignmentOptions.Center);

                GameObject triggerObj = new GameObject("EntryTrigger");
                triggerObj.transform.position =
                    new Vector3(0, ROOM_HEIGHT / 2f, -ROOM_DEPTH / 2f + 1.5f);
                triggerObj.transform.SetParent(envRoot.transform);
                BoxCollider triggerCol = triggerObj.AddComponent<BoxCollider>();
                triggerCol.isTrigger = true;
                triggerCol.size = new Vector3(CORRIDOR_WIDTH + 1f, ROOM_HEIGHT, 2f);
                TunnelIntro intro = triggerObj.AddComponent<TunnelIntro>();

                SerializedObject introSo = new SerializedObject(intro);
                introSo.FindProperty("entryDoor").objectReferenceValue = entryDoorScript;
                introSo.FindProperty("alarmPanel").objectReferenceValue = alarmBg;
                introSo.ApplyModifiedPropertiesWithoutUndo();
            }
        }

        // -- Set interactable layer --
        SetupLayers();

        // Tag terminals as Interactable layer
        foreach (var t in terminalObjects)
        {
            SetLayerRecursive(t, LayerMask.NameToLayer("Interactable"));
        }

        // Update player interaction layer mask
        PlayerInteraction pi = player.GetComponentInChildren<PlayerInteraction>();
        if (pi != null)
        {
            SerializedObject piSo = new SerializedObject(pi);
            piSo.FindProperty("interactableLayer").intValue = 1 << LayerMask.NameToLayer("Interactable");
            piSo.ApplyModifiedPropertiesWithoutUndo();
        }

        // -- Save --
        string scenePath = "Assets/Scenes/GameScene.unity";
        EditorSceneManager.SaveScene(scene, scenePath);
        Debug.Log("GameScene built and saved to " + scenePath);
    }

    // ─────────────────────────────────────────────
    //  BUILD ROOM
    // ─────────────────────────────────────────────

    static GameObject BuildRoom(int index, Vector3 center, Transform parent)
    {
        GameObject room = new GameObject($"Room_{index}_{ROOM_NAMES[index].Replace(" ", "")}");
        room.transform.position = center;
        room.transform.SetParent(parent);

        float hw = ROOM_WIDTH / 2f;
        float hh = ROOM_HEIGHT / 2f;
        float hd = ROOM_DEPTH / 2f;
        float wt = WALL_THICKNESS;
        float floorYLift = (index == 0) ? 0.14f : 0f;

        // Floor (room 0 = lava — slightly higher to avoid z-fighting / wall seam glitches)
        Material floorMat = (index == 0) ? matLava : matFloor;
        CreateBox("Floor", room.transform,
            new Vector3(0, -hh + floorYLift, 0), new Vector3(ROOM_WIDTH, wt, ROOM_DEPTH), floorMat);

        // Ceiling
        CreateBox("Ceiling", room.transform,
            new Vector3(0, hh, 0), new Vector3(ROOM_WIDTH, wt, ROOM_DEPTH), matCeiling);

        // Left wall
        CreateBox("Wall_Left", room.transform,
            new Vector3(-hw, 0, 0), new Vector3(wt, ROOM_HEIGHT, ROOM_DEPTH), matWall);

        // Right wall
        CreateBox("Wall_Right", room.transform,
            new Vector3(hw, 0, 0), new Vector3(wt, ROOM_HEIGHT, ROOM_DEPTH), matWall);

        // Back wall (all rooms have corridor opening for entry/corridor)
        {
            float sideWidth = (ROOM_WIDTH - CORRIDOR_WIDTH) / 2f;
            CreateBox("Wall_Back_L", room.transform,
                new Vector3(-(CORRIDOR_WIDTH / 2f + sideWidth / 2f), 0, -hd),
                new Vector3(sideWidth, ROOM_HEIGHT, wt), matWall);
            CreateBox("Wall_Back_R", room.transform,
                new Vector3(CORRIDOR_WIDTH / 2f + sideWidth / 2f, 0, -hd),
                new Vector3(sideWidth, ROOM_HEIGHT, wt), matWall);
            CreateBox("Wall_Back_Top", room.transform,
                new Vector3(0, hh - 0.5f, -hd),
                new Vector3(CORRIDOR_WIDTH, 1f, wt), matWall);
        }

        // Front wall (with opening for corridor)
        {
            float sideWidth = (ROOM_WIDTH - CORRIDOR_WIDTH) / 2f;
            CreateBox("Wall_Front_L", room.transform,
                new Vector3(-(CORRIDOR_WIDTH / 2f + sideWidth / 2f), 0, hd),
                new Vector3(sideWidth, ROOM_HEIGHT, wt), matWall);
            CreateBox("Wall_Front_R", room.transform,
                new Vector3(CORRIDOR_WIDTH / 2f + sideWidth / 2f, 0, hd),
                new Vector3(sideWidth, ROOM_HEIGHT, wt), matWall);
            CreateBox("Wall_Front_Top", room.transform,
                new Vector3(0, hh - 0.5f, hd),
                new Vector3(CORRIDOR_WIDTH, 1f, wt), matWall);
        }

        // Neon trim strips on walls
        Color accent = ROOM_ACCENT_COLORS[index];
        Material accentMat = index switch
        {
            0 => matNeonGreen,
            1 => matNeonCyan,
            2 => matNeonMagenta,
            3 => matNeonRed,
            4 => matNeonBlue,
            _ => matNeonCyan
        };

        // Floor trim left
        CreateBox("Trim_L", room.transform,
            new Vector3(-hw + wt + 0.05f, -hh + floorYLift + wt + 0.05f, 0),
            new Vector3(0.05f, 0.05f, ROOM_DEPTH - 0.5f), accentMat);

        // Floor trim right
        CreateBox("Trim_R", room.transform,
            new Vector3(hw - wt - 0.05f, -hh + floorYLift + wt + 0.05f, 0),
            new Vector3(0.05f, 0.05f, ROOM_DEPTH - 0.5f), accentMat);

        // Ceiling trim
        CreateBox("Trim_Ceiling", room.transform,
            new Vector3(0, hh - wt - 0.02f, 0),
            new Vector3(ROOM_WIDTH - 1f, 0.03f, 0.03f), accentMat);

        // Room number on back wall (visual marker)
        CreateBox($"RoomMarker", room.transform,
            new Vector3(-hw + wt + 0.5f, 0.5f, -hd + wt + 0.02f),
            new Vector3(0.8f, 0.8f, 0.02f), accentMat);

        // Main room light
        GameObject lightObj = new GameObject($"Light_Room_{index}");
        lightObj.transform.SetParent(room.transform);
        lightObj.transform.localPosition = new Vector3(0, hh - 0.5f, 0);
        Light light = lightObj.AddComponent<Light>();
        light.type = LightType.Point;
        light.color = accent;
        light.intensity = 15f;
        light.range = ROOM_WIDTH * 1.2f;
        light.shadows = LightShadows.Soft;

        // Secondary accent lights
        for (int side = -1; side <= 1; side += 2)
        {
            GameObject accentLight = new GameObject($"AccentLight_{(side < 0 ? "L" : "R")}");
            accentLight.transform.SetParent(room.transform);
            accentLight.transform.localPosition = new Vector3(side * (hw - 1f), -hh + floorYLift + 1f, 0);
            Light al = accentLight.AddComponent<Light>();
            al.type = LightType.Point;
            al.color = accent * 0.5f;
            al.intensity = 5f;
            al.range = 4f;
        }

        // Room 2 (Binary Decode): wall labels — binary for CODE on left, pipeline hint on right
        if (index == 2)
            BuildBinaryDecodeRoomWallText(room.transform, hw, wt);

        // Room 0: lava with 3 small platforms (3 = Caesar shift key)
        if (index == 0)
        {
            CreateBox("EntranceLedge", room.transform,
                new Vector3(0, -hh + floorYLift + 0.15f, -hd + 0.5f),
                new Vector3(CORRIDOR_WIDTH, WALL_THICKNESS, 1f), matFloor);
            CreateBox("Platform_1", room.transform,
                new Vector3(0, -hh + floorYLift + 0.4f, -2.5f),
                new Vector3(1.5f, 0.6f, 1.5f), matFloor);
            CreateBox("Platform_2", room.transform,
                new Vector3(1.2f, -hh + floorYLift + 0.4f, 0.5f),
                new Vector3(1.5f, 0.6f, 1.5f), matFloor);
            CreateBox("Platform_3", room.transform,
                new Vector3(-0.5f, -hh + floorYLift + 0.4f, 3.0f),
                new Vector3(1.5f, 0.6f, 1.5f), matFloor);

            // Lava respawn: remove floor collider so player falls through lava
            Transform floorT = room.transform.Find("Floor");
            if (floorT != null)
            {
                Collider floorCol = floorT.GetComponent<Collider>();
                if (floorCol != null)
                    Object.DestroyImmediate(floorCol);
            }

            // Trigger zone below the visual floor to catch falling players
            GameObject lavaZone = new GameObject("LavaZone");
            lavaZone.transform.SetParent(room.transform);
            lavaZone.transform.localPosition = new Vector3(0, -hh + floorYLift - 0.62f, 0);
            BoxCollider lavaCol = lavaZone.AddComponent<BoxCollider>();
            lavaCol.isTrigger = true;
            lavaCol.size = new Vector3(ROOM_WIDTH + 2f, 1f, ROOM_DEPTH + 2f);
            LavaRespawnZone lrz = lavaZone.AddComponent<LavaRespawnZone>();
            SerializedObject lrzSo = new SerializedObject(lrz);
            lrzSo.FindProperty("respawnPosition").vector3Value =
                new Vector3(0, 1f, center.z - hd + 1f);
            lrzSo.ApplyModifiedPropertiesWithoutUndo();
        }

        return room;
    }

    // ─────────────────────────────────────────────
    //  BUILD CORRIDOR
    // ─────────────────────────────────────────────

    static void BuildCorridor(int index, Vector3 center, Transform parent)
    {
        GameObject corridor = new GameObject($"Corridor_{index}_{index + 1}");
        corridor.transform.position = center;
        corridor.transform.SetParent(parent);

        float hw = CORRIDOR_WIDTH / 2f;
        float hh = ROOM_HEIGHT / 2f;
        float hl = CORRIDOR_LENGTH / 2f;
        float wt = WALL_THICKNESS;

        CreateBox("Floor", corridor.transform,
            Vector3.down * hh, new Vector3(CORRIDOR_WIDTH, wt, CORRIDOR_LENGTH), matFloor);
        CreateBox("Ceiling", corridor.transform,
            Vector3.up * (hh - 0.5f), new Vector3(CORRIDOR_WIDTH, wt, CORRIDOR_LENGTH), matCeiling);
        CreateBox("Wall_L", corridor.transform,
            new Vector3(-hw, -0.25f, 0), new Vector3(wt, ROOM_HEIGHT - 0.5f, CORRIDOR_LENGTH), matWall);
        CreateBox("Wall_R", corridor.transform,
            new Vector3(hw, -0.25f, 0), new Vector3(wt, ROOM_HEIGHT - 0.5f, CORRIDOR_LENGTH), matWall);

        // Neon strip on floor center
        CreateBox("Trim_Floor", corridor.transform,
            new Vector3(0, -hh + wt + 0.02f, 0),
            new Vector3(0.08f, 0.02f, CORRIDOR_LENGTH), matNeonCyan);

        // Corridor light
        GameObject lightObj = new GameObject("CorridorLight");
        lightObj.transform.SetParent(corridor.transform);
        lightObj.transform.localPosition = new Vector3(0, hh - 0.8f, 0);
        Light l = lightObj.AddComponent<Light>();
        l.type = LightType.Point;
        l.color = new Color(0f, 0.6f, 0.8f);
        l.intensity = 6f;
        l.range = CORRIDOR_LENGTH * 1.5f;
    }

    // ─────────────────────────────────────────────
    //  BUILD DOOR
    // ─────────────────────────────────────────────

    static GameObject BuildDoor(int index, Vector3 position)
    {
        GameObject doorRoot = new GameObject($"Door_{index}");
        doorRoot.transform.position = position;

        // Door frame
        float frameW = CORRIDOR_WIDTH + 0.4f;
        float frameH = ROOM_HEIGHT;
        CreateBox("Frame_L", doorRoot.transform,
            new Vector3(-(CORRIDOR_WIDTH / 2f + 0.1f), frameH / 2f, 0),
            new Vector3(0.2f, frameH, 0.3f), matDoorFrame);
        CreateBox("Frame_R", doorRoot.transform,
            new Vector3(CORRIDOR_WIDTH / 2f + 0.1f, frameH / 2f, 0),
            new Vector3(0.2f, frameH, 0.3f), matDoorFrame);
        CreateBox("Frame_Top", doorRoot.transform,
            new Vector3(0, frameH, 0),
            new Vector3(frameW, 0.2f, 0.3f), matDoorFrame);

        // Sliding door panel
        GameObject doorPanel = CreateBox("DoorPanel", doorRoot.transform,
            new Vector3(0, frameH / 2f, 0),
            new Vector3(CORRIDOR_WIDTH, frameH - 0.2f, 0.15f), matDoor);

        // Door light indicator
        GameObject doorLightObj = new GameObject("DoorLight");
        doorLightObj.transform.SetParent(doorRoot.transform);
        doorLightObj.transform.localPosition = new Vector3(0, frameH + 0.1f, 0.2f);
        Light doorLight = doorLightObj.AddComponent<Light>();
        doorLight.type = LightType.Point;
        doorLight.color = Color.red;
        doorLight.intensity = 3f;
        doorLight.range = 2f;

        // Small light indicator box
        CreateBox("Indicator", doorRoot.transform,
            new Vector3(0, frameH + 0.05f, 0.2f),
            new Vector3(0.3f, 0.1f, 0.1f), matNeonRed);

        // Door script
        Door doorScript = doorRoot.AddComponent<Door>();
        SerializedObject so = new SerializedObject(doorScript);
        so.FindProperty("doorTransform").objectReferenceValue = doorPanel.transform;
        so.FindProperty("closedPosition").vector3Value = new Vector3(0, frameH / 2f, 0);
        so.FindProperty("openPosition").vector3Value = new Vector3(0, frameH + frameH / 2f, 0);
        so.FindProperty("openSpeed").floatValue = 1.2f;
        so.FindProperty("doorRenderer").objectReferenceValue = doorPanel.GetComponent<Renderer>();
        so.FindProperty("lockedMaterial").objectReferenceValue = matDoor;
        so.FindProperty("unlockedMaterial").objectReferenceValue = matNeonGreen;
        so.FindProperty("doorLight").objectReferenceValue = doorLight;
        so.FindProperty("lockedLightColor").colorValue = Color.red;
        so.FindProperty("unlockedLightColor").colorValue = Color.green;
        so.ApplyModifiedPropertiesWithoutUndo();

        return doorRoot;
    }

    // ─────────────────────────────────────────────
    //  BUILD TERMINAL
    // ─────────────────────────────────────────────

    static GameObject BuildTerminal(int roomIndex, Vector3 roomCenter)
    {
        GameObject terminal = new GameObject($"Terminal_{roomIndex}");

        float hd = ROOM_DEPTH / 2f;
        terminal.transform.position = roomCenter + new Vector3(0, -ROOM_HEIGHT / 2f, hd - 1.5f);

        // Terminal body (desk)
        CreateBox("Body", terminal.transform,
            new Vector3(0, 0.5f, 0), new Vector3(1.2f, 1f, 0.6f), matTerminalBody);

        // Screen
        GameObject screen = CreateBox("Screen", terminal.transform,
            new Vector3(0, 1.3f, -0.15f), new Vector3(1f, 0.7f, 0.05f), matTerminalScreen);

        // Screen frame
        CreateBox("ScreenFrame", terminal.transform,
            new Vector3(0, 1.3f, -0.18f), new Vector3(1.1f, 0.8f, 0.02f), matTerminalBody);

        // Screen glow light
        GameObject screenLight = new GameObject("ScreenLight");
        screenLight.transform.SetParent(terminal.transform);
        screenLight.transform.localPosition = new Vector3(0, 1.3f, 0.2f);
        Light sl = screenLight.AddComponent<Light>();
        sl.type = LightType.Point;
        sl.color = ROOM_ACCENT_COLORS[roomIndex];
        sl.intensity = 3f;
        sl.range = 2f;

        // Keyboard
        CreateBox("Keyboard", terminal.transform,
            new Vector3(0, 1.02f, 0.15f), new Vector3(0.8f, 0.04f, 0.3f), matTerminalBody);

        // Collider for interaction (larger trigger)
        BoxCollider trigger = terminal.AddComponent<BoxCollider>();
        trigger.isTrigger = false;
        trigger.center = new Vector3(0, 0.8f, 0);
        trigger.size = new Vector3(1.5f, 1.8f, 1.2f);

        // Puzzle Terminal script
        PuzzleTerminal pt = terminal.AddComponent<PuzzleTerminal>();
        SerializedObject ptSo = new SerializedObject(pt);
        ptSo.FindProperty("screenRenderer").objectReferenceValue = screen.GetComponent<Renderer>();
        ptSo.FindProperty("activeScreenMaterial").objectReferenceValue = matTerminalScreen;
        ptSo.FindProperty("inactiveScreenMaterial").objectReferenceValue = matTerminalScreenOff;
        ptSo.FindProperty("terminalLight").objectReferenceValue = sl;
        ptSo.ApplyModifiedPropertiesWithoutUndo();

        // Attach puzzle component based on room
        PuzzleBase puzzle = null;
        switch (roomIndex)
        {
            case 0:
                puzzle = terminal.AddComponent<CaesarCipherPuzzle>();
                SetPuzzleBaseFields(puzzle, "Caesar Cipher",
                    "A movement pattern reveals a constant alphabet shift.", 5);
                break;
            case 1:
                puzzle = terminal.AddComponent<VigenereCipherPuzzle>();
                SetPuzzleBaseFields(puzzle, "Vigenere Cipher",
                    "A repeating keyword controls the shifts.", 5);
                break;
            case 2:
                puzzle = terminal.AddComponent<BinaryDecodePuzzle>();
                SetPuzzleBaseFields(puzzle, "Binary Decode",
                    "Walls show bits and the decoding path; the terminal speaks in riddles.", 5);
                break;
            case 3:
                puzzle = terminal.AddComponent<IPBinaryPuzzle>();
                SetPuzzleBaseFields(puzzle, "IP <-> Binary",
                    "Walls show Part 1 (IPv4) and Part 2 (binary); the terminal explains each step.", 5);
                break;
            case 4:
                puzzle = terminal.AddComponent<DefinitionsPuzzle>();
                SetPuzzleBaseFields(puzzle, "Cyber Definitions Quiz",
                    "Complete missing terms from incident notes.", 5);
                break;
        }

        // Link puzzle to terminal
        if (puzzle != null)
        {
            ptSo = new SerializedObject(pt);
            ptSo.FindProperty("linkedPuzzle").objectReferenceValue = puzzle;
            ptSo.ApplyModifiedPropertiesWithoutUndo();
        }

        // Room 0: terminal faces the player (toward -Z)
        if (roomIndex == 0)
            terminal.transform.rotation = Quaternion.Euler(0, 180f, 0);

        return terminal;
    }

    static void SetPuzzleBaseFields(PuzzleBase puzzle, string name, string desc, int maxAttempts)
    {
        SerializedObject so = new SerializedObject(puzzle);
        so.FindProperty("puzzleName").stringValue = name;
        so.FindProperty("puzzleDescription").stringValue = desc;
        so.FindProperty("maxAttempts").intValue = maxAttempts;
        so.ApplyModifiedPropertiesWithoutUndo();
    }

    // ─────────────────────────────────────────────
    //  BUILD PLAYER
    // ─────────────────────────────────────────────

    static GameObject CreatePlayer()
    {
        GameObject player = new GameObject("Player");
        player.tag = "Player";

        CharacterController cc = player.AddComponent<CharacterController>();
        cc.height = 1.8f;
        cc.radius = 0.3f;
        cc.center = new Vector3(0, 0.9f, 0);

        player.AddComponent<PlayerController>();
        player.AddComponent<PlayerInteraction>();
        player.AddComponent<PlayerBlobShadow>();

        // Camera holder
        GameObject camHolder = new GameObject("CameraHolder");
        camHolder.transform.SetParent(player.transform);
        camHolder.transform.localPosition = new Vector3(0, 1.6f, 0);

        // Camera (tagged MainCamera so Camera.main works)
        camHolder.tag = "MainCamera";
        Camera cam = camHolder.AddComponent<Camera>();
        cam.fieldOfView = 70f;
        cam.nearClipPlane = 0.1f;
        cam.farClipPlane = 100f;
        cam.clearFlags = CameraClearFlags.SolidColor;
        cam.backgroundColor = new Color(0.01f, 0.01f, 0.02f);

        // URP camera data
        var camData = camHolder.AddComponent<UniversalAdditionalCameraData>();
        camData.renderPostProcessing = true;

        // Audio listener
        camHolder.AddComponent<AudioListener>();

        // Wire camera to controller
        PlayerController pc = player.GetComponent<PlayerController>();
        SerializedObject pcSo = new SerializedObject(pc);
        pcSo.FindProperty("cameraHolder").objectReferenceValue = camHolder.transform;
        pcSo.ApplyModifiedPropertiesWithoutUndo();

        return player;
    }

    // ─────────────────────────────────────────────
    //  BUILD EXIT AREA
    // ─────────────────────────────────────────────

    static void BuildExitArea(Vector3 center, Transform parent)
    {
        GameObject exit = new GameObject("ExitArea");
        exit.transform.position = center;
        exit.transform.SetParent(parent);

        float size = 10f;
        float hh = ROOM_HEIGHT / 2f;

        Vector3 exitFloorCenter = Vector3.down * hh;
        GameObject exitDanceFloor = CreateBox("Floor", exit.transform,
            exitFloorCenter, new Vector3(size, WALL_THICKNESS, size), matFloor);
        BuildDiscoTileFloor(exit.transform, exitFloorCenter, size, size, WALL_THICKNESS, "ExitDiscoTiles");
        // Safety platform to avoid any "void fall" feeling
        CreateBox("OuterFloor", exit.transform,
            new Vector3(0, -hh - 0.2f, 0), new Vector3(size + 6f, WALL_THICKNESS, size + 6f), matFloor);
        CreateBox("Ceiling", exit.transform,
            Vector3.up * hh, new Vector3(size, WALL_THICKNESS, size), matCeiling);
        CreateBox("Wall_L", exit.transform,
            new Vector3(-size / 2f, 0, 0), new Vector3(WALL_THICKNESS, ROOM_HEIGHT, size), matWall);
        CreateBox("Wall_R", exit.transform,
            new Vector3(size / 2f, 0, 0), new Vector3(WALL_THICKNESS, ROOM_HEIGHT, size), matWall);
        CreateBox("Wall_Back", exit.transform,
            new Vector3(0, 0, size / 2f), new Vector3(size, ROOM_HEIGHT, WALL_THICKNESS), matWall);

        // Back opening (entry from last corridor)
        float sideW = (size - CORRIDOR_WIDTH) / 2f;
        CreateBox("Wall_Entry_L", exit.transform,
            new Vector3(-(CORRIDOR_WIDTH / 2f + sideW / 2f), 0, -size / 2f),
            new Vector3(sideW, ROOM_HEIGHT, WALL_THICKNESS), matWall);
        CreateBox("Wall_Entry_R", exit.transform,
            new Vector3(CORRIDOR_WIDTH / 2f + sideW / 2f, 0, -size / 2f),
            new Vector3(sideW, ROOM_HEIGHT, WALL_THICKNESS), matWall);

        // Victory lights
        for (int i = 0; i < 6; i++)
        {
            float angle = i * 60f * Mathf.Deg2Rad;
            Vector3 pos = new Vector3(Mathf.Cos(angle) * 2.5f, hh - 0.5f, Mathf.Sin(angle) * 2.5f);
            GameObject vLight = new GameObject($"VictoryLight_{i}");
            vLight.transform.SetParent(exit.transform);
            vLight.transform.localPosition = pos;
            Light l = vLight.AddComponent<Light>();
            l.type = LightType.Point;
            l.color = Color.HSVToRGB(i / 6f, 0.8f, 1f);
            l.intensity = 8f;
            l.range = 5f;
        }

        // "EXIT" sign neon
        CreateBox("ExitSign", exit.transform,
            new Vector3(0, hh - 0.5f, size / 2f - 0.5f),
            new Vector3(2f, 0.5f, 0.05f), matNeonGreen);

        // Trophy pedestal (stepped)
        CreateBox("Pedestal_Base", exit.transform,
            new Vector3(0, -hh + 0.2f, 1.5f), new Vector3(2.2f, 0.4f, 2.2f), matTerminalBody);
        CreateBox("Pedestal_Step", exit.transform,
            new Vector3(0, -hh + 0.55f, 1.5f), new Vector3(1.8f, 0.3f, 1.8f), matTerminalBody);
        CreateBox("Pedestal_Top", exit.transform,
            new Vector3(0, -hh + 0.85f, 1.5f), new Vector3(1.4f, 0.3f, 1.4f), matNeonCyan);

        // Trophy cup
        CreateBox("Trophy_Stem", exit.transform,
            new Vector3(0, -hh + 1.2f, 1.5f), new Vector3(0.25f, 0.4f, 0.25f), matNeonGreen);
        CreateBox("Trophy_Cup", exit.transform,
            new Vector3(0, -hh + 1.7f, 1.5f), new Vector3(0.9f, 0.7f, 0.9f), matNeonGreen);
        CreateBox("Trophy_Rim", exit.transform,
            new Vector3(0, -hh + 2.1f, 1.5f), new Vector3(1.1f, 0.1f, 1.1f), matNeonGreen);
        CreateBox("Trophy_Handle_L", exit.transform,
            new Vector3(-0.65f, -hh + 1.7f, 1.5f), new Vector3(0.15f, 0.35f, 0.15f), matNeonGreen);
        CreateBox("Trophy_Handle_R", exit.transform,
            new Vector3(0.65f, -hh + 1.7f, 1.5f), new Vector3(0.15f, 0.35f, 0.15f), matNeonGreen);

        // Trophy glow
        GameObject trophyLight = new GameObject("TrophyLight");
        trophyLight.transform.SetParent(exit.transform);
        trophyLight.transform.localPosition = new Vector3(0, -hh + 2.5f, 1.5f);
        Light tl = trophyLight.AddComponent<Light>();
        tl.type = LightType.Point;
        tl.color = new Color(0.2f, 1f, 0.4f);
        tl.intensity = 15f;
        tl.range = 5f;
    }

    // ─────────────────────────────────────────────
    //  BUILD ENTRY AREA
    // ─────────────────────────────────────────────

    static void BuildEntryArea(Transform parent)
    {
        GameObject entry = new GameObject("EntryArea");
        entry.transform.position = Vector3.zero;
        entry.transform.SetParent(parent);

        float roomBackZ = -ROOM_DEPTH / 2f;
        float corridorDepth = 4f;
        float corridorStartZ = roomBackZ - corridorDepth;
        float hh = ROOM_HEIGHT / 2f;
        float chw = CORRIDOR_WIDTH / 2f;
        float wt = WALL_THICKNESS;

        // Outside platform (very large, walls prevent void fall)
        Vector3 lobbyFloorCenter = new Vector3(0, 0, -16f);
        GameObject outsideFloor = CreateBox("OutsideFloor", entry.transform,
            lobbyFloorCenter, new Vector3(40f, wt, 24f), matFloor);
        BuildDiscoTileFloor(entry.transform, lobbyFloorCenter, 40f, 24f, wt, "LobbyDiscoTiles");
        BuildDiscoBall(entry.transform, new Vector3(0f, 5.2f, -16f));
        CreateBox("OutsideWall_L", entry.transform,
            new Vector3(-20f, 3f, -16f), new Vector3(wt, 6f, 24f), matWall);
        CreateBox("OutsideWall_R", entry.transform,
            new Vector3(20f, 3f, -16f), new Vector3(wt, 6f, 24f), matWall);
        CreateBox("OutsideWall_Back", entry.transform,
            new Vector3(0, 3f, -28f), new Vector3(40f, 6f, wt), matWall);
        CreateBox("OutsideWall_Front_L", entry.transform,
            new Vector3(-10.75f, 3f, corridorStartZ),
            new Vector3(18.5f, 6f, wt), matWall);
        CreateBox("OutsideWall_Front_R", entry.transform,
            new Vector3(10.75f, 3f, corridorStartZ),
            new Vector3(18.5f, 6f, wt), matWall);

        // Entry corridor geometry (Z from corridorStartZ to roomBackZ)
        float ecZ = (corridorStartZ + roomBackZ) / 2f;

        CreateBox("EntryCorridor_Floor", entry.transform,
            new Vector3(0, 0, ecZ),
            new Vector3(CORRIDOR_WIDTH, wt, corridorDepth), matFloor);
        CreateBox("EntryCorridor_Ceiling", entry.transform,
            new Vector3(0, ROOM_HEIGHT, ecZ),
            new Vector3(CORRIDOR_WIDTH, wt, corridorDepth), matCeiling);
        CreateBox("EntryCorridor_Wall_L", entry.transform,
            new Vector3(-chw, hh, ecZ),
            new Vector3(wt, ROOM_HEIGHT, corridorDepth), matWall);
        CreateBox("EntryCorridor_Wall_R", entry.transform,
            new Vector3(chw, hh, ecZ),
            new Vector3(wt, ROOM_HEIGHT, corridorDepth), matWall);

        // Neon strip on corridor floor
        CreateBox("CorridorTrim", entry.transform,
            new Vector3(0, wt + 0.02f, ecZ),
            new Vector3(0.08f, 0.02f, corridorDepth), matNeonCyan);

        // Arch pillars at tunnel entrance
        CreateBox("Arch_L", entry.transform,
            new Vector3(-chw - 0.3f, hh, corridorStartZ),
            new Vector3(0.4f, ROOM_HEIGHT + 0.5f, 0.4f), matNeonCyan);
        CreateBox("Arch_R", entry.transform,
            new Vector3(chw + 0.3f, hh, corridorStartZ),
            new Vector3(0.4f, ROOM_HEIGHT + 0.5f, 0.4f), matNeonCyan);
        CreateBox("Arch_Top", entry.transform,
            new Vector3(0, ROOM_HEIGHT + 0.5f, corridorStartZ),
            new Vector3(CORRIDOR_WIDTH + 1f, 0.4f, 0.4f), matNeonMagenta);

        // Minecraft-style sign in front of entrance
        float signZ = corridorStartZ - 1.5f;
        CreateBox("SignPost", entry.transform,
            new Vector3(0, 0.7f, signZ), new Vector3(0.12f, 1.4f, 0.12f), matTerminalBody);
        CreateBox("SignBoard", entry.transform,
            new Vector3(0, 1.55f, signZ), new Vector3(0.95f, 0.6f, 0.07f), matTerminalBody);

        GameObject signCanvas = new GameObject("SignCanvas");
        signCanvas.transform.SetParent(entry.transform);
        signCanvas.transform.position = new Vector3(0, 1.55f, signZ - 0.04f);
        signCanvas.transform.rotation = Quaternion.Euler(0, 180f, 0);
        Canvas sc = signCanvas.AddComponent<Canvas>();
        sc.renderMode = RenderMode.WorldSpace;
        RectTransform scRT = signCanvas.GetComponent<RectTransform>();
        scRT.sizeDelta = new Vector2(200, 130);
        scRT.localScale = new Vector3(0.003f, 0.003f, 0.003f);

        GameObject signText = new GameObject("SignText");
        signText.transform.SetParent(signCanvas.transform);
        RectTransform stRT = signText.AddComponent<RectTransform>();
        stRT.anchorMin = Vector2.zero;
        stRT.anchorMax = Vector2.one;
        stRT.offsetMin = Vector2.zero;
        stRT.offsetMax = Vector2.zero;
        stRT.anchoredPosition = new Vector2(-6f, 1f);
        signText.transform.localPosition = new Vector3(0, 0, 8f);
        signText.transform.localScale = new Vector3(2f, 2f, 2f);
        TextMeshProUGUI stTmp = signText.AddComponent<TextMeshProUGUI>();
        stTmp.text = "GRAND\n\nPRIZE\n\nINSIDE";
        stTmp.fontSize = 20;
        stTmp.color = new Color(0.756f, 1f, 0f);
        stTmp.fontStyle = FontStyles.Bold;
        stTmp.alignment = TextAlignmentOptions.Center;
        stTmp.margin = new Vector4(0, 0, 12.375f, 0);

        // Sign glow light
        GameObject signLight = new GameObject("SignLight");
        signLight.transform.SetParent(entry.transform);
        signLight.transform.position =
            new Vector3(0, ROOM_HEIGHT + 0.8f, corridorStartZ - 1.5f);
        Light sl = signLight.AddComponent<Light>();
        sl.type = LightType.Point;
        sl.color = new Color(1f, 0.6f, 0.1f);
        sl.intensity = 8f;
        sl.range = 5f;

        // Ambient lights outside
        for (int side = -1; side <= 1; side += 2)
        {
            GameObject outsideLight = new GameObject($"OutsideLight_{side}");
            outsideLight.transform.SetParent(entry.transform);
            outsideLight.transform.position = new Vector3(side * 8f, 2f, -16f);
            Light ol = outsideLight.AddComponent<Light>();
            ol.type = LightType.Point;
            ol.color = new Color(0.1f, 0.3f, 0.6f);
            ol.intensity = 5f;
            ol.range = 10f;
        }
    }

    // ─────────────────────────────────────────────
    //  BUILD GAME UI
    // ─────────────────────────────────────────────

    static void BuildGameUI(Transform parent, List<Room> rooms)
    {
        // --- HUD Canvas ---
        GameObject hudCanvas = CreateCanvas("HUD_Canvas", parent, 1);
        HUDManager hud = hudCanvas.AddComponent<HUDManager>();

        // Crosshair
        GameObject crosshair = CreateUIImage("Crosshair", hudCanvas.transform,
            Vector2.zero, new Vector2(4, 4), Color.white);

        // Timer (top-right corner)
        GameObject timerObj = CreateUIText("TimerText", hudCanvas.transform,
            Vector2.zero, new Vector2(200, 40), "00:00", 24,
            Color.white, TextAlignmentOptions.Right);
        RectTransform timerRT = timerObj.GetComponent<RectTransform>();
        timerRT.anchorMin = new Vector2(1f, 1f);
        timerRT.anchorMax = new Vector2(1f, 1f);
        timerRT.pivot = new Vector2(1f, 1f);
        timerRT.anchoredPosition = new Vector2(-20f, -15f);

        // Room text (top-left corner)
        GameObject roomTextObj = CreateUIText("RoomText", hudCanvas.transform,
            Vector2.zero, new Vector2(260, 40), "Room 1/5", 20,
            Color.white, TextAlignmentOptions.Left);
        RectTransform roomRT = roomTextObj.GetComponent<RectTransform>();
        roomRT.anchorMin = new Vector2(0f, 1f);
        roomRT.anchorMax = new Vector2(0f, 1f);
        roomRT.pivot = new Vector2(0f, 1f);
        roomRT.anchoredPosition = new Vector2(20f, -15f);

        // Interact prompt
        GameObject promptObj = CreateUIText("InteractPrompt", hudCanvas.transform,
            new Vector2(0, -80), new Vector2(300, 40), "Press [E] to interact", 18,
            Color.white, TextAlignmentOptions.Center);
        promptObj.SetActive(false);

        // Room indicators
        GameObject indicatorsParent = new GameObject("RoomIndicators");
        indicatorsParent.transform.SetParent(hudCanvas.transform);
        RectTransform indRT = indicatorsParent.AddComponent<RectTransform>();
        indRT.anchorMin = new Vector2(0.5f, 1f);
        indRT.anchorMax = new Vector2(0.5f, 1f);
        indRT.anchoredPosition = new Vector2(0, -60);
        indRT.sizeDelta = new Vector2(200, 20);

        HorizontalLayoutGroup hlg = indicatorsParent.AddComponent<HorizontalLayoutGroup>();
        hlg.spacing = 8;
        hlg.childAlignment = TextAnchor.MiddleCenter;
        hlg.childControlWidth = false;
        hlg.childControlHeight = false;

        GameObject[] indicators = new GameObject[5];
        for (int i = 0; i < 5; i++)
        {
            Color col = i == 0 ? Color.yellow : Color.red;
            indicators[i] = CreateUIImage($"Indicator_{i}", indicatorsParent.transform,
                Vector2.zero, new Vector2(20, 20), col);
        }

        // Wire HUD
        SerializedObject hudSo = new SerializedObject(hud);
        hudSo.FindProperty("timerText").objectReferenceValue = timerObj.GetComponent<TextMeshProUGUI>();
        hudSo.FindProperty("roomText").objectReferenceValue = roomTextObj.GetComponent<TextMeshProUGUI>();
        hudSo.FindProperty("interactPromptText").objectReferenceValue = promptObj.GetComponent<TextMeshProUGUI>();
        hudSo.FindProperty("crosshair").objectReferenceValue = crosshair;

        SerializedProperty indArray = hudSo.FindProperty("roomIndicators");
        indArray.arraySize = 5;
        for (int i = 0; i < 5; i++)
            indArray.GetArrayElementAtIndex(i).objectReferenceValue = indicators[i];

        hudSo.ApplyModifiedPropertiesWithoutUndo();

        // --- Completion Panel (under HUD canvas) ---
        GameObject completionPanel = CreatePanel("CompletionPanel", hudCanvas.transform,
            new Color(0f, 0f, 0f, 0.9f));
        completionPanel.SetActive(false);

        CreateUIText("CompTitle", completionPanel.transform,
            new Vector2(0, 100), new Vector2(600, 60), "TUNNEL CLEARED", 42,
            new Color(0f, 1f, 0.5f), TextAlignmentOptions.Center);

        GameObject compTime = CreateUIText("CompTime", completionPanel.transform,
            new Vector2(0, 20), new Vector2(500, 40), "Time: 00:00", 28,
            Color.white, TextAlignmentOptions.Center);

        GameObject compMsg = CreateUIText("CompMessage", completionPanel.transform,
            new Vector2(0, -40), new Vector2(500, 40), "Rank: ELITE HACKER", 24,
            new Color(1f, 0.8f, 0f), TextAlignmentOptions.Center);

        hudSo = new SerializedObject(hud);
        hudSo.FindProperty("completionPanel").objectReferenceValue = completionPanel;
        hudSo.FindProperty("completionTimeText").objectReferenceValue = compTime.GetComponent<TextMeshProUGUI>();
        hudSo.FindProperty("completionMessageText").objectReferenceValue = compMsg.GetComponent<TextMeshProUGUI>();
        hudSo.ApplyModifiedPropertiesWithoutUndo();

        // --- PUZZLE UI Canvas ---
        GameObject puzzleCanvas = CreateCanvas("PuzzleUI_Canvas", parent, 2);
        PuzzleUIManager pui = puzzleCanvas.AddComponent<PuzzleUIManager>();

        // Main puzzle panel
        GameObject puzzlePanel = CreatePanel("PuzzlePanel", puzzleCanvas.transform,
            new Color(0.02f, 0.05f, 0.08f, 0.95f));
        puzzlePanel.SetActive(false);

        // Puzzle title
        GameObject titleObj = CreateUIText("PuzzleTitle", puzzlePanel.transform,
            new Vector2(0, 300), new Vector2(900, 44), "[ PUZZLE NAME ]", 28,
            new Color(0f, 1f, 0.6f), TextAlignmentOptions.Center);

        // Puzzle content (tall + wide so Vigenère / long puzzles fit; no ellipsis)
        GameObject contentObj = CreateUIText("PuzzleContent", puzzlePanel.transform,
            new Vector2(0, 8), new Vector2(900, 460), "Puzzle content here...", 18,
            new Color(0.7f, 0.9f, 0.7f), TextAlignmentOptions.TopLeft);
        {
            var contentTmp = contentObj.GetComponent<TextMeshProUGUI>();
            contentTmp.overflowMode = TextOverflowModes.Overflow;
            contentTmp.alignment = TextAlignmentOptions.TopLeft;
            contentTmp.margin = new Vector4(12f, 8f, 12f, 8f);
        }

        // Feedback text
        GameObject feedbackObj = CreateUIText("FeedbackText", puzzlePanel.transform,
            new Vector2(0, -248), new Vector2(860, 56), "", 20,
            Color.green, TextAlignmentOptions.Center);

        // Answer input
        GameObject inputObj = CreateInputField("AnswerInput", puzzlePanel.transform,
            new Vector2(0, -302), new Vector2(520, 44));

        // Submit button
        GameObject submitBtn = CreateButton("SubmitButton", puzzlePanel.transform,
            new Vector2(-95, -362), new Vector2(160, 44), "SUBMIT",
            new Color(0f, 0.6f, 0.3f));

        // Hint button
        GameObject hintBtn = CreateButton("HintButton", puzzlePanel.transform,
            new Vector2(95, -362), new Vector2(160, 44), "HINT",
            new Color(0.3f, 0.3f, 0.6f));

        // Close button
        GameObject closeBtn = CreateButton("CloseButton", puzzlePanel.transform,
            new Vector2(448, 312), new Vector2(48, 48), "X",
            new Color(0.6f, 0.1f, 0.1f));

        // Multiple choice panel
        GameObject mcPanel = new GameObject("MultipleChoicePanel");
        mcPanel.transform.SetParent(puzzlePanel.transform);
        RectTransform mcRT = mcPanel.AddComponent<RectTransform>();
        mcRT.anchorMin = mcRT.anchorMax = new Vector2(0.5f, 0.5f);
        mcRT.anchoredPosition = new Vector2(0, -150);
        mcRT.sizeDelta = new Vector2(560, 280);
        mcPanel.SetActive(false);

        VerticalLayoutGroup vlg = mcPanel.AddComponent<VerticalLayoutGroup>();
        vlg.spacing = 10;
        vlg.childAlignment = TextAnchor.MiddleCenter;
        vlg.childControlWidth = true;
        vlg.childControlHeight = false;
        vlg.childForceExpandWidth = true;

        Button[] choiceBtns = new Button[4];
        TextMeshProUGUI[] choiceTexts = new TextMeshProUGUI[4];
        Color[] btnColors = {
            new Color(0f, 0.4f, 0.5f), new Color(0.4f, 0.2f, 0.5f),
            new Color(0.1f, 0.4f, 0.3f), new Color(0.4f, 0.35f, 0.1f)
        };

        for (int i = 0; i < 4; i++)
        {
            GameObject btn = CreateButton($"Choice_{i}", mcPanel.transform,
                Vector2.zero, new Vector2(400, 45), $"Option {i + 1}", btnColors[i]);
            LayoutElement le = btn.AddComponent<LayoutElement>();
            le.preferredHeight = 45;
            choiceBtns[i] = btn.GetComponent<Button>();
            choiceTexts[i] = btn.GetComponentInChildren<TextMeshProUGUI>();
        }

        // Definitions panel
        GameObject defPanel = new GameObject("DefinitionsPanel");
        defPanel.transform.SetParent(puzzlePanel.transform);
        RectTransform defRT = defPanel.AddComponent<RectTransform>();
        defRT.anchorMin = defRT.anchorMax = new Vector2(0.5f, 0.5f);
        defRT.anchoredPosition = new Vector2(0, -80);
        defRT.sizeDelta = new Vector2(600, 250);
        defPanel.SetActive(false);

        GameObject defText = CreateUIText("DefinitionText", defPanel.transform,
            new Vector2(0, 50), new Vector2(550, 100),
            "Definition placeholder...", 18,
            new Color(0.8f, 0.9f, 0.8f), TextAlignmentOptions.Center);

        GameObject defInput = CreateInputField("DefinitionInput", defPanel.transform,
            new Vector2(0, -30), new Vector2(350, 45));

        GameObject defSubmit = CreateButton("DefSubmitButton", defPanel.transform,
            new Vector2(0, -90), new Vector2(180, 45), "SUBMIT",
            new Color(0f, 0.5f, 0.4f));

        GameObject defProgress = CreateUIText("DefProgress", defPanel.transform,
            new Vector2(0, -130), new Vector2(500, 30), "Progress: 0/3 needed", 16,
            new Color(0.5f, 0.7f, 1f), TextAlignmentOptions.Center);

        // Hint panel (fixed-size, positioned at bottom of puzzle area)
        GameObject hintPanel = new GameObject("HintPanel");
        hintPanel.transform.SetParent(puzzlePanel.transform);
        RectTransform hintRT = hintPanel.AddComponent<RectTransform>();
        hintRT.anchorMin = hintRT.anchorMax = new Vector2(0.5f, 0.5f);
        hintRT.anchoredPosition = new Vector2(0, -380);
        hintRT.sizeDelta = new Vector2(820, 120);
        Image hintBgImg = hintPanel.AddComponent<Image>();
        hintBgImg.color = new Color(0.04f, 0.04f, 0.12f, 0.96f);
        UnityEngine.UI.Outline hintOutline = hintPanel.AddComponent<UnityEngine.UI.Outline>();
        hintOutline.effectColor = new Color(0.8f, 0.8f, 0.3f, 0.5f);
        hintOutline.effectDistance = new Vector2(2, 2);
        hintPanel.SetActive(false);

        GameObject hintTextObj = CreateUIText("HintText", hintPanel.transform,
            new Vector2(0, 4), new Vector2(780, 100), "Hint text...", 15,
            new Color(0.9f, 0.9f, 0.5f), TextAlignmentOptions.Center);

        GameObject hintCloseBtn = CreateButton("HintCloseButton", hintPanel.transform,
            new Vector2(392, 30), new Vector2(22, 22), "X",
            new Color(0.55f, 0.12f, 0.12f));
        var hintCloseLabel = hintCloseBtn.GetComponentInChildren<TextMeshProUGUI>();
        if (hintCloseLabel != null)
            hintCloseLabel.fontSize = 13;

        // Wire PuzzleUIManager
        SerializedObject puiSo = new SerializedObject(pui);
        puiSo.FindProperty("puzzlePanel").objectReferenceValue = puzzlePanel;
        puiSo.FindProperty("puzzleTitleText").objectReferenceValue = titleObj.GetComponent<TextMeshProUGUI>();
        puiSo.FindProperty("puzzleContentText").objectReferenceValue = contentObj.GetComponent<TextMeshProUGUI>();
        puiSo.FindProperty("feedbackText").objectReferenceValue = feedbackObj.GetComponent<TextMeshProUGUI>();
        puiSo.FindProperty("answerInput").objectReferenceValue = inputObj.GetComponent<TMP_InputField>();
        puiSo.FindProperty("submitButton").objectReferenceValue = submitBtn.GetComponent<Button>();
        puiSo.FindProperty("hintButton").objectReferenceValue = hintBtn.GetComponent<Button>();
        puiSo.FindProperty("closeButton").objectReferenceValue = closeBtn.GetComponent<Button>();
        puiSo.FindProperty("multipleChoicePanel").objectReferenceValue = mcPanel;

        SerializedProperty cbArray = puiSo.FindProperty("choiceButtons");
        cbArray.arraySize = 4;
        for (int i = 0; i < 4; i++)
            cbArray.GetArrayElementAtIndex(i).objectReferenceValue = choiceBtns[i];

        SerializedProperty ctArray = puiSo.FindProperty("choiceTexts");
        ctArray.arraySize = 4;
        for (int i = 0; i < 4; i++)
            ctArray.GetArrayElementAtIndex(i).objectReferenceValue = choiceTexts[i];

        puiSo.FindProperty("definitionsPanel").objectReferenceValue = defPanel;
        puiSo.FindProperty("definitionText").objectReferenceValue = defText.GetComponent<TextMeshProUGUI>();
        puiSo.FindProperty("definitionInput").objectReferenceValue = defInput.GetComponent<TMP_InputField>();
        puiSo.FindProperty("definitionSubmitButton").objectReferenceValue = defSubmit.GetComponent<Button>();
        puiSo.FindProperty("definitionProgressText").objectReferenceValue = defProgress.GetComponent<TextMeshProUGUI>();
        puiSo.FindProperty("hintPanel").objectReferenceValue = hintPanel;
        puiSo.FindProperty("hintText").objectReferenceValue = hintTextObj.GetComponent<TextMeshProUGUI>();
        puiSo.FindProperty("hintCloseButton").objectReferenceValue = hintCloseBtn.GetComponent<Button>();

        // Fun Facts Panel (centered popup with dim background)
        GameObject ffDimBg = CreatePanel("FunFactsDimBg", puzzleCanvas.transform,
            new Color(0f, 0f, 0f, 0.6f));
        ffDimBg.SetActive(false);

        GameObject funFactsPopup = new GameObject("FunFactsPopup");
        funFactsPopup.transform.SetParent(ffDimBg.transform);
        RectTransform ffPopRT = funFactsPopup.AddComponent<RectTransform>();
        ffPopRT.anchorMin = ffPopRT.anchorMax = new Vector2(0.5f, 0.5f);
        ffPopRT.anchoredPosition = Vector2.zero;
        ffPopRT.sizeDelta = new Vector2(620, 420);
        Image ffPopBg = funFactsPopup.AddComponent<Image>();
        ffPopBg.color = new Color(0.03f, 0.07f, 0.12f, 0.98f);
        UnityEngine.UI.Outline ffOutline = funFactsPopup.AddComponent<UnityEngine.UI.Outline>();
        ffOutline.effectColor = new Color(0f, 1f, 0.5f, 0.5f);
        ffOutline.effectDistance = new Vector2(3, 3);

        CreateUIText("FunFactsTitle", funFactsPopup.transform,
            new Vector2(0, 155), new Vector2(560, 55), "ACCESS GRANTED", 34,
            new Color(0f, 1f, 0.5f), TextAlignmentOptions.Center);

        CreateUIImage("FunFactsDivider", funFactsPopup.transform,
            new Vector2(0, 118), new Vector2(480, 2), new Color(0f, 1f, 0.5f, 0.4f));

        GameObject ffTextObj = CreateUIText("FunFactsBody", funFactsPopup.transform,
            new Vector2(0, -15), new Vector2(560, 230), "", 17,
            new Color(0.85f, 0.92f, 0.85f), TextAlignmentOptions.Center);

        GameObject ffBtn = CreateButton("FunFactsContinue", funFactsPopup.transform,
            new Vector2(0, -180), new Vector2(200, 48), "CONTINUE",
            new Color(0f, 0.5f, 0.3f));

        puiSo.FindProperty("funFactsPanel").objectReferenceValue = ffDimBg;
        puiSo.FindProperty("funFactsText").objectReferenceValue =
            ffTextObj.GetComponent<TextMeshProUGUI>();
        puiSo.FindProperty("funFactsContinueButton").objectReferenceValue =
            ffBtn.GetComponent<Button>();
        puiSo.ApplyModifiedPropertiesWithoutUndo();

        // --- PAUSE MENU Canvas ---
        GameObject pauseCanvas = CreateCanvas("PauseMenu_Canvas", parent, 3);
        PauseMenuUI pauseUI = pauseCanvas.AddComponent<PauseMenuUI>();

        GameObject pausePanel = CreatePanel("PausePanel", pauseCanvas.transform,
            new Color(0f, 0f, 0f, 0.85f));
        pausePanel.SetActive(false);

        CreateUIText("PauseTitle", pausePanel.transform,
            new Vector2(0, 120), new Vector2(400, 60), "PAUSED", 40,
            Color.white, TextAlignmentOptions.Center);

        GameObject resumeBtn = CreateButton("ResumeBtn", pausePanel.transform,
            new Vector2(0, 40), new Vector2(250, 50), "RESUME",
            new Color(0f, 0.5f, 0.3f));
        GameObject restartBtn = CreateButton("RestartBtn", pausePanel.transform,
            new Vector2(0, -20), new Vector2(250, 50), "RESTART",
            new Color(0.4f, 0.4f, 0.1f));
        GameObject menuBtn = CreateButton("MainMenuBtn", pausePanel.transform,
            new Vector2(0, -80), new Vector2(250, 50), "MAIN MENU",
            new Color(0.3f, 0.3f, 0.5f));
        GameObject quitBtn = CreateButton("QuitBtn", pausePanel.transform,
            new Vector2(0, -140), new Vector2(250, 50), "QUIT",
            new Color(0.5f, 0.1f, 0.1f));

        SerializedObject pauseSo = new SerializedObject(pauseUI);
        pauseSo.FindProperty("pausePanel").objectReferenceValue = pausePanel;
        pauseSo.FindProperty("resumeButton").objectReferenceValue = resumeBtn.GetComponent<Button>();
        pauseSo.FindProperty("restartButton").objectReferenceValue = restartBtn.GetComponent<Button>();
        pauseSo.FindProperty("mainMenuButton").objectReferenceValue = menuBtn.GetComponent<Button>();
        pauseSo.FindProperty("quitButton").objectReferenceValue = quitBtn.GetComponent<Button>();
        pauseSo.ApplyModifiedPropertiesWithoutUndo();

        // Wire interact prompt to player
        var players = Object.FindObjectsByType<PlayerInteraction>(FindObjectsSortMode.None);
        if (players.Length > 0)
        {
            SerializedObject piSo = new SerializedObject(players[0]);
            piSo.FindProperty("interactionPrompt").objectReferenceValue = promptObj;
            piSo.ApplyModifiedPropertiesWithoutUndo();
        }
    }

    // ─────────────────────────────────────────────
    //  MAIN MENU SCENE
    // ─────────────────────────────────────────────

    static void BuildMainMenuScene()
    {
        var scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
        scene.name = "MainMenu";

        RenderSettings.ambientMode = UnityEngine.Rendering.AmbientMode.Flat;
        RenderSettings.ambientLight = new Color(0.01f, 0.01f, 0.02f);

        // Camera
        GameObject camObj = new GameObject("MainCamera");
        Camera cam = camObj.AddComponent<Camera>();
        cam.clearFlags = CameraClearFlags.SolidColor;
        cam.backgroundColor = new Color(0.02f, 0.02f, 0.05f);
        camObj.AddComponent<UniversalAdditionalCameraData>();
        camObj.AddComponent<AudioListener>();

        // Scene Loader (persistent)
        GameObject slObj = new GameObject("SceneLoader");
        slObj.AddComponent<SceneLoader>();

        // Audio Manager
        GameObject audioObj = new GameObject("AudioManager");
        audioObj.AddComponent<AudioManager>();
        AudioSource ms = audioObj.AddComponent<AudioSource>();
        ms.loop = true;
        ms.volume = 0.3f;
        AudioSource sfx = audioObj.AddComponent<AudioSource>();

        SerializedObject amSo = new SerializedObject(audioObj.GetComponent<AudioManager>());
        amSo.FindProperty("musicSource").objectReferenceValue = ms;
        amSo.FindProperty("sfxSource").objectReferenceValue = sfx;
        amSo.ApplyModifiedPropertiesWithoutUndo();

        // Canvas
        GameObject canvas = CreateCanvas("MenuCanvas", null, 0);
        MainMenuUI menuUI = canvas.AddComponent<MainMenuUI>();

        // Background
        CreatePanel("Background", canvas.transform, new Color(0.02f, 0.03f, 0.06f, 1f));

        // Decorative lines
        for (int i = 0; i < 8; i++)
        {
            float y = -300 + i * 85;
            Color lineColor = new Color(0f, 0.3f + i * 0.05f, 0.5f + i * 0.03f, 0.15f);
            CreateUIImage($"Line_{i}", canvas.transform,
                new Vector2(0, y), new Vector2(1920, 1), lineColor);
        }

        // -- Main Panel --
        GameObject mainPanel = new GameObject("MainPanel");
        mainPanel.transform.SetParent(canvas.transform);
        RectTransform mpRT = mainPanel.AddComponent<RectTransform>();
        mpRT.anchorMin = mpRT.anchorMax = new Vector2(0.5f, 0.5f);
        mpRT.anchoredPosition = Vector2.zero;
        mpRT.sizeDelta = new Vector2(600, 500);

        CreateUIText("Title", mainPanel.transform,
            new Vector2(0, 180), new Vector2(600, 80), "CYBERTUNNEL", 56,
            new Color(0f, 1f, 0.6f), TextAlignmentOptions.Center);

        CreateUIText("Subtitle", mainPanel.transform,
            new Vector2(0, 120), new Vector2(600, 40),
            "Cybersecurity Escape Challenge", 20,
            new Color(0.4f, 0.7f, 0.9f), TextAlignmentOptions.Center);

        GameObject startBtn = CreateButton("StartButton", mainPanel.transform,
            new Vector2(0, 30), new Vector2(300, 55), "START GAME",
            new Color(0f, 0.5f, 0.3f));

        GameObject settingsBtn = CreateButton("SettingsButton", mainPanel.transform,
            new Vector2(0, -35), new Vector2(300, 55), "SETTINGS",
            new Color(0.2f, 0.3f, 0.5f));

        GameObject creditsBtn = CreateButton("CreditsButton", mainPanel.transform,
            new Vector2(0, -100), new Vector2(300, 55), "CREDITS",
            new Color(0.3f, 0.25f, 0.45f));

        GameObject quitBtn = CreateButton("QuitButton", mainPanel.transform,
            new Vector2(0, -165), new Vector2(300, 55), "QUIT",
            new Color(0.5f, 0.1f, 0.1f));

        // Version text
        CreateUIText("Version", mainPanel.transform,
            new Vector2(0, -230), new Vector2(300, 25), "v1.0 - University Project", 14,
            new Color(0.3f, 0.3f, 0.4f), TextAlignmentOptions.Center);

        // -- Settings Panel --
        GameObject settingsPanel = new GameObject("SettingsPanel");
        settingsPanel.transform.SetParent(canvas.transform);
        RectTransform spRT = settingsPanel.AddComponent<RectTransform>();
        spRT.anchorMin = spRT.anchorMax = new Vector2(0.5f, 0.5f);
        spRT.anchoredPosition = Vector2.zero;
        spRT.sizeDelta = new Vector2(500, 400);
        settingsPanel.SetActive(false);

        CreateUIText("SettingsTitle", settingsPanel.transform,
            new Vector2(0, 150), new Vector2(400, 50), "SETTINGS", 36,
            new Color(0f, 0.8f, 1f), TextAlignmentOptions.Center);

        CreateUIText("MusicLabel", settingsPanel.transform,
            new Vector2(-100, 70), new Vector2(150, 30), "Music Volume", 16,
            Color.white, TextAlignmentOptions.MidlineRight);

        GameObject musicSliderObj = CreateSlider("MusicSlider", settingsPanel.transform,
            new Vector2(80, 70), new Vector2(200, 20));

        CreateUIText("SFXLabel", settingsPanel.transform,
            new Vector2(-100, 20), new Vector2(150, 30), "SFX Volume", 16,
            Color.white, TextAlignmentOptions.MidlineRight);

        GameObject sfxSliderObj = CreateSlider("SFXSlider", settingsPanel.transform,
            new Vector2(80, 20), new Vector2(200, 20));

        GameObject backSettingsBtn = CreateButton("BackSettings", settingsPanel.transform,
            new Vector2(0, -120), new Vector2(200, 45), "BACK",
            new Color(0.3f, 0.3f, 0.4f));

        // -- Credits Panel --
        GameObject creditsPanel = new GameObject("CreditsPanel");
        creditsPanel.transform.SetParent(canvas.transform);
        RectTransform cpRT = creditsPanel.AddComponent<RectTransform>();
        cpRT.anchorMin = cpRT.anchorMax = new Vector2(0.5f, 0.5f);
        cpRT.anchoredPosition = Vector2.zero;
        cpRT.sizeDelta = new Vector2(500, 400);
        creditsPanel.SetActive(false);

        CreateUIText("CreditsTitle", creditsPanel.transform,
            new Vector2(0, 150), new Vector2(400, 50), "CREDITS", 36,
            new Color(0f, 0.8f, 1f), TextAlignmentOptions.Center);

        CreateUIText("CreditsBody", creditsPanel.transform,
            new Vector2(0, 20), new Vector2(400, 200),
            "CYBERTUNNEL\n\nDeveloped by:\n- Alexandru Nagy\n- Alexandra Satalan\n- Cristian Rusu\n- Robert Peternel\n- Andrei Apostol\n\nUniversity Project - GUI - 2026\nBuilt with Unity URP",
            16, new Color(0.7f, 0.8f, 0.9f), TextAlignmentOptions.Center);

        GameObject backCreditsBtn = CreateButton("BackCredits", creditsPanel.transform,
            new Vector2(0, -150), new Vector2(200, 45), "BACK",
            new Color(0.3f, 0.3f, 0.4f));

        // Wire MainMenuUI
        SerializedObject muiSo = new SerializedObject(menuUI);
        muiSo.FindProperty("mainPanel").objectReferenceValue = mainPanel;
        muiSo.FindProperty("settingsPanel").objectReferenceValue = settingsPanel;
        muiSo.FindProperty("creditsPanel").objectReferenceValue = creditsPanel;
        muiSo.FindProperty("startButton").objectReferenceValue = startBtn.GetComponent<Button>();
        muiSo.FindProperty("settingsButton").objectReferenceValue = settingsBtn.GetComponent<Button>();
        muiSo.FindProperty("creditsButton").objectReferenceValue = creditsBtn.GetComponent<Button>();
        muiSo.FindProperty("quitButton").objectReferenceValue = quitBtn.GetComponent<Button>();
        muiSo.FindProperty("backFromSettingsButton").objectReferenceValue = backSettingsBtn.GetComponent<Button>();
        muiSo.FindProperty("backFromCreditsButton").objectReferenceValue = backCreditsBtn.GetComponent<Button>();
        muiSo.FindProperty("musicVolumeSlider").objectReferenceValue = musicSliderObj.GetComponent<Slider>();
        muiSo.FindProperty("sfxVolumeSlider").objectReferenceValue = sfxSliderObj.GetComponent<Slider>();
        muiSo.ApplyModifiedPropertiesWithoutUndo();

        // Ambient decorative lights
        for (int i = 0; i < 3; i++)
        {
            GameObject dLight = new GameObject($"DecoLight_{i}");
            dLight.transform.position = new Vector3(-3 + i * 3, 2, -5);
            Light l = dLight.AddComponent<Light>();
            l.type = LightType.Point;
            l.color = Color.HSVToRGB(i * 0.33f, 0.7f, 0.5f);
            l.intensity = 3f;
            l.range = 8f;
        }

        string scenePath = "Assets/Scenes/MainMenu.unity";
        EditorSceneManager.SaveScene(scene, scenePath);
        Debug.Log("MainMenu scene built and saved to " + scenePath);
    }

    // ─────────────────────────────────────────────
    //  BUILD SETTINGS
    // ─────────────────────────────────────────────

    static void SetupBuildSettings()
    {
        var scenes = new List<EditorBuildSettingsScene>
        {
            new EditorBuildSettingsScene("Assets/Scenes/MainMenu.unity", true),
            new EditorBuildSettingsScene("Assets/Scenes/GameScene.unity", true)
        };
        EditorBuildSettings.scenes = scenes.ToArray();
        Debug.Log("Build settings updated with MainMenu and GameScene.");
    }

    static void SetupLayers()
    {
        SerializedObject tagManager = new SerializedObject(
            AssetDatabase.LoadAllAssetsAtPath("ProjectSettings/TagManager.asset")[0]);

        SerializedProperty layers = tagManager.FindProperty("layers");

        bool found = false;
        for (int i = 8; i < layers.arraySize; i++)
        {
            if (layers.GetArrayElementAtIndex(i).stringValue == "Interactable")
            {
                found = true;
                break;
            }
        }

        if (!found)
        {
            for (int i = 8; i < layers.arraySize; i++)
            {
                if (string.IsNullOrEmpty(layers.GetArrayElementAtIndex(i).stringValue))
                {
                    layers.GetArrayElementAtIndex(i).stringValue = "Interactable";
                    tagManager.ApplyModifiedPropertiesWithoutUndo();
                    Debug.Log($"Created 'Interactable' layer at index {i}");
                    break;
                }
            }
        }
    }

    static void SetLayerRecursive(GameObject obj, int layer)
    {
        obj.layer = layer;
        foreach (Transform child in obj.transform)
            SetLayerRecursive(child.gameObject, layer);
    }

    // ─────────────────────────────────────────────
    //  HELPER: GEOMETRY
    // ─────────────────────────────────────────────

    static void CreateDirectionalLight()
    {
        GameObject lightObj = new GameObject("DirectionalLight");
        lightObj.transform.rotation = Quaternion.Euler(50f, -30f, 0f);
        Light light = lightObj.AddComponent<Light>();
        light.type = LightType.Directional;
        light.color = new Color(0.05f, 0.08f, 0.15f);
        light.intensity = 0.3f;
        light.shadows = LightShadows.Soft;
    }

    static void BuildDiscoTileFloor(Transform parent, Vector3 floorCenterLocal, float widthX, float depthZ,
        float floorThicknessY, string tilesRootName)
    {
        if (matDiscoTile == null) return;

        const float tileSize = 2f;
        const float tileH = 0.08f;
        float halfW = widthX * 0.5f;
        float halfD = depthZ * 0.5f;
        float ty = floorCenterLocal.y + floorThicknessY * 0.5f + tileH * 0.5f;

        GameObject root = new GameObject(tilesRootName);
        root.transform.SetParent(parent, false);
        root.transform.localPosition = Vector3.zero;

        int ix = 0;
        for (float wx = -halfW + tileSize * 0.5f; wx < halfW - 0.001f; wx += tileSize, ix++)
        {
            int iz = 0;
            for (float wz = -halfD + tileSize * 0.5f; wz < halfD - 0.001f; wz += tileSize)
            {
                Vector3 lp = new Vector3(floorCenterLocal.x + wx, ty, floorCenterLocal.z + wz);
                GameObject tile = GameObject.CreatePrimitive(PrimitiveType.Cube);
                tile.name = $"DiscoTile_{ix}_{iz}";
                tile.transform.SetParent(root.transform, false);
                tile.transform.localPosition = lp;
                float s = tileSize * 0.98f;
                tile.transform.localScale = new Vector3(s, tileH, s);
                tile.isStatic = true;
                tile.GetComponent<Renderer>().sharedMaterial = matDiscoTile;

                DiscoTile dt = tile.AddComponent<DiscoTile>();
                dt.Configure(ix, iz);
                iz++;
            }
        }
    }

    static void BuildDiscoBall(Transform parent, Vector3 localPos)
    {
        if (matDiscoBall == null) return;

        GameObject root = new GameObject("DiscoBall");
        root.transform.SetParent(parent, false);
        root.transform.localPosition = localPos;

        GameObject sphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        sphere.name = "BallMesh";
        sphere.transform.SetParent(root.transform, false);
        sphere.transform.localPosition = Vector3.zero;
        sphere.transform.localScale = Vector3.one * 1.2f;
        Object.DestroyImmediate(sphere.GetComponent<Collider>());
        sphere.GetComponent<Renderer>().sharedMaterial = matDiscoBall;

        GameObject pivot = new GameObject("LightPivot");
        pivot.transform.SetParent(root.transform, false);
        pivot.transform.localPosition = Vector3.zero;

        float[] hues = { 0f, 0.28f, 0.55f, 0.78f };
        for (int i = 0; i < hues.Length; i++)
        {
            GameObject lg = new GameObject($"DiscoSpot_{i}");
            lg.transform.SetParent(pivot.transform, false);
            float a = i * Mathf.PI * 0.5f;
            lg.transform.localPosition = new Vector3(Mathf.Cos(a) * 2f, 0f, Mathf.Sin(a) * 2f);
            Light L = lg.AddComponent<Light>();
            L.type = LightType.Point;
            L.color = Color.HSVToRGB(hues[i], 0.95f, 1f);
            L.intensity = 1.8f;
            L.range = 12f;
        }

        DiscoBall db = root.AddComponent<DiscoBall>();
        db.SetLightPivot(pivot.transform);
    }

    static GameObject CreateBox(string name, Transform parent, Vector3 localPos, Vector3 scale, Material mat)
    {
        GameObject obj = GameObject.CreatePrimitive(PrimitiveType.Cube);
        obj.name = name;
        obj.transform.SetParent(parent);
        obj.transform.localPosition = localPos;
        obj.transform.localScale = scale;
        obj.isStatic = true;

        if (mat != null)
            obj.GetComponent<Renderer>().sharedMaterial = mat;

        return obj;
    }

    /// <summary>IP/Binary room (index 3): Part 1 IPv4 on left wall, Part 2 binary on right.</summary>
    static void BuildIPBinaryRoomWallText(Transform room, IPBinaryPuzzle puzzle)
    {
        float hw = ROOM_WIDTH / 2f;
        float wt = WALL_THICKNESS;
        float inset = wt * 0.5f + 0.08f;

        CreateWallTextMeshPro("Wall_IP_Part1_Left", room,
            new Vector3(-hw + inset, 0.55f, 0f),
            Quaternion.Euler(0f, -90f, 0f),
            new Vector2(8.5f, 1.9f),
            puzzle.GetPart1WallLabel(), 2.15f, new Color(0.95f, 0.55f, 1f), TextAlignmentOptions.Center);

        CreateWallTextMeshPro("Wall_IP_Part2_Right", room,
            new Vector3(hw - inset, 0.55f, 0f),
            Quaternion.Euler(0f, 90f, 0f),
            new Vector2(9f, 2.4f),
            puzzle.GetPart2WallLabel(), 1.65f, new Color(0.35f, 0.92f, 1f), TextAlignmentOptions.Center);
    }

    /// <summary>Binary Decode room (index 2): 3D TMP flush on left/right walls (avoids broken WorldSpace Canvas scale).</summary>
    static void BuildBinaryDecodeRoomWallText(Transform room, float hw, float wt)
    {
        const string binaryLine = "01000011 01001111 01000100 01000101";
        const string pipeline = "Binary → Decimal → ASCII → Text.";

        float inset = wt * 0.5f + 0.08f;
        // Y rotation faces the *front* of TMP toward room interior (was 90 / -90 → mirrored glyphs).
        CreateWallTextMeshPro("Wall_BinaryCode_Left", room,
            new Vector3(-hw + inset, 0.55f, 0f),
            Quaternion.Euler(0f, -90f, 0f),
            new Vector2(7.8f, 1.35f),
            binaryLine, 2.6f, new Color(0.95f, 0.55f, 1f), TextAlignmentOptions.Center);

        CreateWallTextMeshPro("Wall_BinaryPipeline_Right", room,
            new Vector3(hw - inset, 0.6f, 0f),
            Quaternion.Euler(0f, 90f, 0f),
            new Vector2(5.2f, 0.95f),
            pipeline, 2.85f, new Color(0.35f, 0.92f, 1f), TextAlignmentOptions.Center);
    }

    /// <summary>Flat TextMeshPro (3D mesh) on wall; rect and fontSize are world meters.</summary>
    static void CreateWallTextMeshPro(string name, Transform parent, Vector3 localPos,
        Quaternion localRot, Vector2 rectSizeMeters, string text, float fontSizeWorld,
        Color color, TextAlignmentOptions align)
    {
        GameObject go = new GameObject(name);
        go.transform.SetParent(parent);
        go.transform.localPosition = localPos;
        go.transform.localRotation = localRot;
        go.transform.localScale = Vector3.one;

        TextMeshPro tmp = go.AddComponent<TextMeshPro>();
        tmp.text = text;
        tmp.fontSize = fontSizeWorld;
        tmp.color = color;
        tmp.fontStyle = FontStyles.Bold;
        tmp.alignment = align;
        tmp.enableWordWrapping = true;
        tmp.overflowMode = TextOverflowModes.Overflow;
        tmp.enableAutoSizing = false;
        tmp.rectTransform.sizeDelta = rectSizeMeters;

        var mr = go.GetComponent<MeshRenderer>();
        if (mr != null)
            mr.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
    }

    // ─────────────────────────────────────────────
    //  HELPER: UI
    // ─────────────────────────────────────────────

    static GameObject CreateCanvas(string name, Transform parent, int sortOrder)
    {
        GameObject canvasObj = new GameObject(name);
        if (parent != null)
            canvasObj.transform.SetParent(parent);

        Canvas canvas = canvasObj.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.sortingOrder = sortOrder;

        CanvasScaler scaler = canvasObj.AddComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1920, 1080);
        scaler.matchWidthOrHeight = 0.5f;

        canvasObj.AddComponent<GraphicRaycaster>();

        // Ensure EventSystem exists
        if (Object.FindFirstObjectByType<UnityEngine.EventSystems.EventSystem>() == null)
        {
            GameObject es = new GameObject("EventSystem");
            es.AddComponent<UnityEngine.EventSystems.EventSystem>();
            es.AddComponent<UnityEngine.EventSystems.StandaloneInputModule>();
        }

        return canvasObj;
    }

    static GameObject CreatePanel(string name, Transform parent, Color color)
    {
        GameObject panel = new GameObject(name);
        panel.transform.SetParent(parent);

        RectTransform rt = panel.AddComponent<RectTransform>();
        rt.anchorMin = Vector2.zero;
        rt.anchorMax = Vector2.one;
        rt.offsetMin = Vector2.zero;
        rt.offsetMax = Vector2.zero;

        Image img = panel.AddComponent<Image>();
        img.color = color;

        return panel;
    }

    static GameObject CreateUIText(string name, Transform parent, Vector2 position,
        Vector2 size, string text, int fontSize, Color color, TextAlignmentOptions align)
    {
        GameObject obj = new GameObject(name);
        obj.transform.SetParent(parent);

        RectTransform rt = obj.AddComponent<RectTransform>();
        rt.anchorMin = rt.anchorMax = new Vector2(0.5f, 0.5f);
        rt.anchoredPosition = position;
        rt.sizeDelta = size;

        TextMeshProUGUI tmp = obj.AddComponent<TextMeshProUGUI>();
        tmp.text = text;
        tmp.fontSize = fontSize;
        tmp.color = color;
        tmp.fontStyle = FontStyles.Normal;
        tmp.enableAutoSizing = false;
        tmp.alignment = align;
        tmp.enableWordWrapping = true;
        tmp.overflowMode = TextOverflowModes.Ellipsis;

        return obj;
    }

    static GameObject CreateUIImage(string name, Transform parent, Vector2 position,
        Vector2 size, Color color)
    {
        GameObject obj = new GameObject(name);
        obj.transform.SetParent(parent);

        RectTransform rt = obj.AddComponent<RectTransform>();
        rt.anchorMin = rt.anchorMax = new Vector2(0.5f, 0.5f);
        rt.anchoredPosition = position;
        rt.sizeDelta = size;

        Image img = obj.AddComponent<Image>();
        img.color = color;

        return obj;
    }

    static GameObject CreateButton(string name, Transform parent, Vector2 position,
        Vector2 size, string text, Color bgColor)
    {
        GameObject btnObj = new GameObject(name);
        btnObj.transform.SetParent(parent);

        RectTransform rt = btnObj.AddComponent<RectTransform>();
        rt.anchorMin = rt.anchorMax = new Vector2(0.5f, 0.5f);
        rt.anchoredPosition = position;
        rt.sizeDelta = size;

        Image img = btnObj.AddComponent<Image>();
        img.color = bgColor;

        Button btn = btnObj.AddComponent<Button>();
        ColorBlock cb = btn.colors;
        cb.normalColor = bgColor;
        cb.highlightedColor = bgColor * 1.3f;
        cb.pressedColor = bgColor * 0.7f;
        cb.selectedColor = bgColor * 1.1f;
        btn.colors = cb;

        GameObject textObj = CreateUIText($"{name}_Text", btnObj.transform,
            Vector2.zero, size, text, 18, Color.white, TextAlignmentOptions.Center);

        return btnObj;
    }

    static GameObject CreateInputField(string name, Transform parent, Vector2 position, Vector2 size)
    {
        GameObject inputObj = new GameObject(name);
        inputObj.transform.SetParent(parent);

        RectTransform rt = inputObj.AddComponent<RectTransform>();
        rt.anchorMin = rt.anchorMax = new Vector2(0.5f, 0.5f);
        rt.anchoredPosition = position;
        rt.sizeDelta = size;

        Image bg = inputObj.AddComponent<Image>();
        bg.color = new Color(0.05f, 0.1f, 0.15f, 0.9f);

        // Text area
        GameObject textArea = new GameObject("Text Area");
        textArea.transform.SetParent(inputObj.transform);
        RectTransform taRT = textArea.AddComponent<RectTransform>();
        taRT.anchorMin = Vector2.zero;
        taRT.anchorMax = Vector2.one;
        taRT.offsetMin = new Vector2(10, 5);
        taRT.offsetMax = new Vector2(-10, -5);

        RectMask2D mask = textArea.AddComponent<RectMask2D>();

        // Placeholder
        GameObject placeholder = new GameObject("Placeholder");
        placeholder.transform.SetParent(textArea.transform);
        RectTransform phRT = placeholder.AddComponent<RectTransform>();
        phRT.anchorMin = Vector2.zero;
        phRT.anchorMax = Vector2.one;
        phRT.offsetMin = Vector2.zero;
        phRT.offsetMax = Vector2.zero;
        TextMeshProUGUI phText = placeholder.AddComponent<TextMeshProUGUI>();
        phText.text = "Type your answer...";
        phText.fontSize = 16;
        phText.color = new Color(0.3f, 0.5f, 0.4f, 0.6f);
        phText.alignment = TextAlignmentOptions.MidlineLeft;

        // Input text
        GameObject inputText = new GameObject("Text");
        inputText.transform.SetParent(textArea.transform);
        RectTransform itRT = inputText.AddComponent<RectTransform>();
        itRT.anchorMin = Vector2.zero;
        itRT.anchorMax = Vector2.one;
        itRT.offsetMin = Vector2.zero;
        itRT.offsetMax = Vector2.zero;
        TextMeshProUGUI text = inputText.AddComponent<TextMeshProUGUI>();
        text.fontSize = 16;
        text.color = new Color(0f, 1f, 0.5f);
        text.alignment = TextAlignmentOptions.MidlineLeft;

        // Caret
        GameObject caret = new GameObject("Caret");
        caret.transform.SetParent(textArea.transform);
        caret.AddComponent<RectTransform>();

        TMP_InputField input = inputObj.AddComponent<TMP_InputField>();
        input.textViewport = taRT;
        input.textComponent = text;
        input.placeholder = phText;
        input.fontAsset = text.font;
        input.pointSize = 16;
        input.caretColor = new Color(0f, 1f, 0.5f);
        input.selectionColor = new Color(0f, 0.5f, 0.3f, 0.4f);
        input.lineType = TMP_InputField.LineType.SingleLine;

        return inputObj;
    }

    static GameObject CreateSlider(string name, Transform parent, Vector2 position, Vector2 size)
    {
        GameObject sliderObj = new GameObject(name);
        sliderObj.transform.SetParent(parent);

        RectTransform rt = sliderObj.AddComponent<RectTransform>();
        rt.anchorMin = rt.anchorMax = new Vector2(0.5f, 0.5f);
        rt.anchoredPosition = position;
        rt.sizeDelta = size;

        // Background
        GameObject bgObj = new GameObject("Background");
        bgObj.transform.SetParent(sliderObj.transform);
        RectTransform bgRT = bgObj.AddComponent<RectTransform>();
        bgRT.anchorMin = new Vector2(0, 0.25f);
        bgRT.anchorMax = new Vector2(1, 0.75f);
        bgRT.offsetMin = Vector2.zero;
        bgRT.offsetMax = Vector2.zero;
        Image bgImg = bgObj.AddComponent<Image>();
        bgImg.color = new Color(0.1f, 0.1f, 0.15f);

        // Fill Area
        GameObject fillArea = new GameObject("Fill Area");
        fillArea.transform.SetParent(sliderObj.transform);
        RectTransform faRT = fillArea.AddComponent<RectTransform>();
        faRT.anchorMin = new Vector2(0, 0.25f);
        faRT.anchorMax = new Vector2(1, 0.75f);
        faRT.offsetMin = Vector2.zero;
        faRT.offsetMax = Vector2.zero;

        GameObject fill = new GameObject("Fill");
        fill.transform.SetParent(fillArea.transform);
        RectTransform fillRT = fill.AddComponent<RectTransform>();
        fillRT.anchorMin = Vector2.zero;
        fillRT.anchorMax = Vector2.one;
        fillRT.offsetMin = Vector2.zero;
        fillRT.offsetMax = Vector2.zero;
        Image fillImg = fill.AddComponent<Image>();
        fillImg.color = new Color(0f, 0.7f, 0.5f);

        // Handle area
        GameObject handleArea = new GameObject("Handle Slide Area");
        handleArea.transform.SetParent(sliderObj.transform);
        RectTransform haRT = handleArea.AddComponent<RectTransform>();
        haRT.anchorMin = Vector2.zero;
        haRT.anchorMax = Vector2.one;
        haRT.offsetMin = new Vector2(10, 0);
        haRT.offsetMax = new Vector2(-10, 0);

        GameObject handle = new GameObject("Handle");
        handle.transform.SetParent(handleArea.transform);
        RectTransform hRT = handle.AddComponent<RectTransform>();
        hRT.sizeDelta = new Vector2(20, 0);
        Image handleImg = handle.AddComponent<Image>();
        handleImg.color = new Color(0f, 1f, 0.6f);

        Slider slider = sliderObj.AddComponent<Slider>();
        slider.fillRect = fillRT;
        slider.handleRect = hRT;
        slider.targetGraphic = handleImg;
        slider.minValue = 0;
        slider.maxValue = 1;
        slider.value = 0.7f;

        return sliderObj;
    }
}
