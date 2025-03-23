# 🎯 Badge Notification System

This system adds **badge notifications** to UI elements in Unity, showing numbers or symbols to indicate updates.

## 🛠️ Setup Instructions

### 1️⃣ Add the `BadgeManager`
1. Create an **empty GameObject** in your scene.
2. Attach the **`BadgeManager`** script to it.
3. You can also drag and drop the badmanager prefab from the `BadgeNotificationSystem` folder into the scene.

### 2️⃣ Attach `BadgeComponent` to Button UI Elements
1. Add the **`BadgeComponent`** script to any UI element (e.g., buttons).
2. Set the **`BadgeID`** to match the badge type (Shop, Quests, Inbox, etc.).
3. Assign a **symbol image** to the image variable.

### 3️⃣ Initialize Badges in `BadgeMappingInitializer`
1. Attach **`BadgeMappingInitializer`** to a GameObject in the scene.
2. Configure **child mappings** in the Inspector.
3. The system will initialize badges and propagate changes automatically.

### 4️⃣ Trigger a Badge Update
To set a badge (e.g., shop):
```csharp
BadgeManager.Instance.SetBadge(BadgeID.Shop);
````
To clear a badge (e.g., remove badge on shop):
```csharp
BadgeManager.Instance.ClearBadge(BadgeID.Shop);
```

You can customize the system to support numerical notifications as well by modifying the BadgeType enum to include more types of badges. 

