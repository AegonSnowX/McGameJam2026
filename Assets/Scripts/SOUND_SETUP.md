# Sound Setup Guide

Use this as a checklist. Add clips/sources where listed; leave optional fields empty if you don’t have that sound yet.

---

## 1. GameAudioManager (UI & global one-shots)

**Script:** `GameManager.cs` folder → **GameAudioManager.cs**  
**Where:** One empty GameObject in your main game scene (e.g. "AudioManager"). Add **GameAudioManager** and one **AudioSource** (no spatial blend; 2D).

| Field | What to put |
|-------|------------------|
| **UI Source** | The AudioSource on this GameObject (for one-shots). |
| **Button Click Clip** | Short click/pop for menu and in-game buttons. |
| **Death Sting Clip** | Sting or impact when the player dies. |
| **Win Sting Clip** | Short sting or fanfare when the player wins. |

**Note:** GameAudioManager uses `DontDestroyOnLoad`, so it persists across scene loads if you have a menu → game flow.

---

## 2. Button click (all UI buttons)

**Script:** **PlaySoundOnButtonClick.cs**  
**Where:** Add this **component to every Button** that should make a click sound.

- Main menu: Start, Credit, Exit  
- Death screen: Restart, Quit  
- Win screen: Restart, Quit  
- Clue panel: Close  

No fields to assign; it uses **GameAudioManager.PlayButtonClick()**. Ensure **GameAudioManager** exists in the scene (and has **Button Click Clip** set).

---

## 3. Key pickup

**Script:** **KeyCollectible.cs** (already has sound support)  
**Where:** On each key GameObject (or key prefab).

| Field | What to put |
|-------|------------------|
| **Collect Sound** | An **AudioSource** (on the key or a child) with your key-pickup clip. |

**Spatial:** For 3D feel, add an AudioSource on the key, set **Spatial Blend = 1**, assign its clip, and drag it into **Collect Sound**.

---

## 4. Clue pickup (open & close)

**Script:** **ClueInteractable.cs**  
**Where:** On each clue GameObject (or clue prefab).

| Field | What to put |
|-------|------------------|
| **Open Sound** | AudioSource (or one-shot source) that plays when the player opens the clue (press E). |
| **Close Sound** | AudioSource that plays when the player closes the clue panel. |

Use short paper/page or UI sounds. Can be 2D (same source as UI) or a small 3D source on the clue (Spatial Blend = 1).

---

## 5. Trap sound

**Script:** **TrapTrigger.cs** (already has sound support)  
**Where:** On each trap GameObject.

| Field | What to put |
|-------|------------------|
| **Trap Sound** | AudioSource that plays when the player triggers the trap (and attracts enemies). |

**Spatial:** Put the AudioSource on the trap and set **Spatial Blend = 1** so it’s 3D and enemies “hear” it from the trap position.

---

## 6. Ghost / enemy sounds (3D)

**Script:** **Enemy.cs**  
**Where:** On the enemy prefab (or each enemy in the scene).

| Field | What to put |
|-------|------------------|
| **Patrol Sound** | Looping ambient/breathing when patrolling. Set **Loop = true**, **Spatial Blend = 1**. |
| **Chase Sound** | Looping tense/chase when chasing the player. **Loop = true**, **Spatial Blend = 1**. |
| **Attack Sound** | One-shot when the enemy attacks (e.g. lunge/hit). **Spatial Blend = 1**. |

**Important:** For each of these AudioSources, set **Spatial Blend = 1** (3D) so volume and pan follow the enemy position. Adjust **Min/Max Distance** on the AudioSource so the ghost is audible at the right range.

---

## 7. Walk / footsteps

**Script:** **PlayerFootstepSound.cs**  
**Where:** On the **player** GameObject (same as PlayerMovement).

| Field | What to put |
|-------|------------------|
| **Footstep Source** | AudioSource on the player (optional; uses same object’s source if empty). |
| **Footstep Clip** | Single footstep clip (script plays it on a timer while moving). |
| **Step Interval** | Seconds between steps (e.g. 0.35–0.5). |
| **Pitch Min / Max** | Small random pitch variation per step (e.g. 0.9–1.1). |

Usually 2D (Spatial Blend = 0). For 3D footsteps, use Spatial Blend = 1 and tune distance.

---

## 8. Heartbeat (noise-driven, like torch flicker)

**Script:** **HeartbeatSound.cs**  
**Where:** On the player (or same object as TorchEffects / MicrophoneInput).

| Field | What to put |
|-------|------------------|
| **Heartbeat Source** | AudioSource (optional; uses same object if empty). Set **Loop = true**. |
| **Heartbeat Clip** | A looping heartbeat (one beat or short loop). |
| **Noise Threshold** | Match **TorchEffects** (e.g. 0.2) so heartbeat and light flicker together. |
| **Pitch Min / Pitch Max** | Low pitch when quiet, higher when loud (e.g. 0.8–1.5). |

Heartbeat volume and pitch increase when the player is loud (microphone above threshold), in line with the torch flicker.

---

## 9. Death and win (already wired)

**Scripts:** **GameManager.cs**  
**Where:** No extra components. GameManager already calls:

- **GameAudioManager.PlayDeathSound()** when the player dies.  
- **GameAudioManager.PlayWinSound()** when the player wins.

Assign **Death Sting Clip** and **Win Sting Clip** on **GameAudioManager** (see section 1).

---

## Quick checklist

| # | Place | Script / Component | What sound |
|---|--------|--------------------|------------|
| 1 | AudioManager GameObject | GameAudioManager | Button click, death sting, win sting (clips + one UI AudioSource) |
| 2 | Every UI Button | PlaySoundOnButtonClick | (no assign – uses GameAudioManager) |
| 3 | Keys | KeyCollectible | Collect sound (AudioSource) |
| 4 | Clues | ClueInteractable | Open sound, close sound |
| 5 | Traps | TrapTrigger | Trap sound (AudioSource, 3D optional) |
| 6 | Enemies | Enemy | Patrol loop, chase loop, attack one-shot (all 3D) |
| 7 | Player | PlayerFootstepSound | Footstep clip + timing |
| 8 | Player | HeartbeatSound | Heartbeat loop + noise threshold |

---

## 3D / spatial reminder

- **Spatial Blend = 0** → 2D (no distance falloff). Use for: UI, button click, death/win, optional footsteps.  
- **Spatial Blend = 1** → 3D (positional). Use for: keys, clues (optional), traps, enemy patrol/chase/attack, optional footsteps.  
- Tune **Min Distance** and **Max Distance** on each 3D AudioSource so sounds are audible at the right range.
