# Design Documentation

# Table of Contents

### Version 1.0

### Game Overview

- Game Log Line
- Gameplay Synopsis

### Game Details

- Description
- Genre
- Platform
- Similar Games

### Audience

- Target Audience
- Player Goals

### Gameplay & General Features

- Block System
- Player Control
- Combo
- Last Stand
- Randomly Gained Skills
- Player Goals
- Characters

### The Game World

- Overview
- Key Locations
- Scale

### Camera

- Overview
- Move
- Shake

### User Interface & Art

- Main Menu
- Battle Background(float)
- Block
- Character
- Actions
- Icons And Other Titles

### Music & Sound Effects

### Multiplayer Game

### Resources & Links

### Reference

---

## Version 1.0

- First version of the document.

Change Log (compare to Beta)

- Fixed bugs: 
    - When using Slime, using skills while hitting causes the hit blink to not go away and further crashes the game.
    - Slime cube sometimes stuck in the bottom of the platform.
    - Slime cubes sometimes change color when thrown.
- Added a new random skill icon and changed the hint to make it more visible.
- The randomization algorithm has been optimized and now everyone's blocks in the opening game will be more balanced.
- A new character have been added.
- Better audio sources replaced the former sources, added audio effect of Combo.

---

## Game Overview

- To gain God's favor, to bring favor to the race, to make a name for yourself, you have come to this arena… In the game Build And Hit (AKA B&H), you can choose your own character and either elevate yourself or destroy the hope of your enemies.
- To achieve triumph and conquer formidable rivals like yourself, one must employ all means and ponder each step with utmost care. Embrace patience and cunning? Retreat strategically to gain an advantage? Wage an all-or-nothing stand? For if we withhold retaliation, our foes shall respond with redoubled force!

### Game Log Line

- You are the most powerful warrior of your race in Gehenna, and you hope to bring glory and wealth to your people through beating your opponent.

### Gameplay Synopsis

- You'll take control of your very own character and use unique character skills and your wits, and maybe a little bit of luck, to destroy your opponent's tower, win the ultimate victory.

---

## Game Details

### Description

- In the game, you can choose your character and play against your opponent with the goal of eliminating all of your opponent's blocks to win. During the battle, you can use your character's unique skills or powerful randomly triggered skills, which are important weapons on your way to victory. Use your intelligence and strategy to win!

### Genre

- Theme: pixel art, cute, 2D
- Game type: strategy, casual, multi-player

### Platform

- PC - WebGL, Windows, Mac

### Similar Games

- Zuma

![Untitled](Design%20Doc/Untitled.png)

1. It is an elimination game, the basic gameplay is to control the frog in the middle of the screen to eliminate the colored balls on the track by shooting the colored balls, when all the colored balls in a level are eliminated that is over, such as all the elimination of the ball into the skull at the end of the track before that is a failure.
2. Our game takes the idea of eliminating colored balls from Zuma and applies it to our game's combat, with the difference that we've placed it in a two-player platform and modified some of the elimination logic, as well as adding more interesting skills and characters to make the experience unique for each player.

---

## Audience

### Target Audience

- The game is an all-ages casual versus game that is easy to play without hardcore operations or complex concepts. Our target audience is everyone who wants to have fun, relieve stress or bond with friends or loved ones.

### Player Goals

- Players will notice the mechanics from the blocks, characters and skills.
- Players will be encouraged to learn more details when they continue to play and gain understanding of game, and create different ways to  take advantage of the characters they choose to win.
- Players will want to play the game again and again not only to win but also to improve the relationship with opponents(in game, friends in reality).

---

## Gameplay & General Features

### Block System

- The block system is **the core of the game**, all the battles are achieved through the block construction and elimination, the key to victory is how to manipulate the blocks, so that player‘s own block tower to stand and destroy the opponent's block tower.
- Blocks come in three colors, red, green and blue, and some characters can build blocks of more **unique** colors through their skills.

![Untitled](Design%20Doc/Untitled%201.png)

![Untitled](Design%20Doc/Untitled%202.png)

![Untitled](Design%20Doc/Untitled%203.png)

![Untitled](Design%20Doc/Untitled%204.png)

- Blocks of the same color interact differently than blocks of different colors, be careful! When there are too many blocks of the same color, it's easy to trigger a combo: a very powerful way to eliminate blocks.

### Player Control

- Build: **P1 : A / P2 : Left** Build a block for your Tower.
- Hit: **P1 : D / P2 : Right** + **Press W/S or Up/Down** + **P1 : D/P2 : Right** Destroy the enemy Tower's block.
- Role skill：**P1 : Q / P2 : <** When the cooldown is over, activate your character's skill.
- Random skill: **P1 : E / P2 : >** Activate a random skill when there is a chance to get a powerful skill.

![Untitled](Design%20Doc/Untitled%205.png)

Keys for player1

![Untitled](Design%20Doc/Untitled%206.png)

Keys for player2

### Combo

- Use the block in your hand to hit the enemy **two squares of the same color & a square of the same color** in your hand for a basic combo.
- Use the blocks in your hand to **three consecutive blocks of the same color** on the enemy's blocks, resulting in an advanced combo.
- When some blocks disappear and the blocks above them fall down, the combo can still be triggered if there are the same three or more blocks.
- And players can see that the blocks will become **transparent** for a period of time before the combo to give players stronger visual feedback.

![Untitled](Design%20Doc/Untitled%207.png)

### Last Stand

- When any player has only one block left, they can build **an extra block** in their next build. For engineer who is good at building, he can use his skills to build an extra block on top of his existing skill builds.
- Each player has only one Last Stand, so please treasure it!

### Randomly Gained Skills

- **Overview**
  
    In addition to the skills of the player's chosen character, the player has a chance to gain a second skill at any given time: the Random Skill. The Random Skill will give the player an advantage at unexpected times, but if used incorrectly, the player can also add insult to injury to their tower. This one skill was designed to make winning the game unpredictable in the long run. We want this game to be more than just a Go-like quest for the best solutions, we want to give players the fun of interacting in a multiplayer game.
    
- **Dynamic probabilistic compensation algorithm**
  
    However, there is a small chance that the player will cause multiple Combos in a round (but this is actually something we encourage players to achieve). A Hit that causes multiple blocks at once will give the attacker a great sense of accomplishment, but comparatively, the defender will have a great sense of loss. In order to simultaneously balance the sense of accomplishment that Combo brings, but without causing too much loss to the player who is lost a number of blocks, we designed a dynamic compensation algorithm for the skill acquisition probability. 
    
    This algorithm has some compensation on the side of the player who lost squares as described above. When a player loses a certain number of blocks in a short period of time, or when the difference between two players is more than a certain number of blocks, the disadvantaged side will significantly increase the probability of skill acquisition. After many tests, players when using skills wisely are able to narrow down some losses, but at the same time will not make the advantageous side reduce the sense of achievement.
    

![Untitled](Design%20Doc/Untitled%208.png)

- Skill List
  
    Skill 1: Destroy your opponent's first block.
    
    Skill 2: Get a block with different color.
    
    Skill 3: Change player’s block color.
    

### Player Goals

1. Completely destroy opponent's block tower.
2. After 20 turns, make sure your tower is higher than your opponent's.

### Characters

- **Overview:**
  
    **B&H** is a PVP game that two players against each other. Each player getting to choose their own character. Each character has their own unique skills and operation styles. Players can build their own advantage based on their chosen character and use their character's skills to defeat the enemy Tower of blocks.
    
- **Character creations:**
    1. Dasher (Bubblegum)
       
        ![Untitled](Design%20Doc/Untitled.jpeg)
        
        1. Description: With blazing red hair and keen eagle eyes, Bubblegum is agile and swift, seeking the perfect balance of speed and strength. Dasher has a better vision. She has more flexibility to maneuver. When Dasher finds that a block under her opponent has the condition for a Combo, she can use skill to hit that block, though that block is far away from herself.
        2. Hit range: 4
        3. Skills: **Eagle Eye**—Increase hit range to 8 blocks for 1 round.
        4. Cool down: 4 round
    2. Slime (Bobo)
       
        ![Untitled](Design%20Doc/Untitled%201.jpeg)
        
        1. Description: In the Otherworld, there lives a soft blue slime. It has a bright blue body and cute eyes that always glow with warmth. Very friendly and understanding, this cute Slime is the guardian of the forest and attracts many adventurers to make new friends. Slime's blue mucus brings his exclusive advantage. It has a unique way of generating cubes: it can use their own mucus as the raw material to build blocks. 
        2. Hit range: 5
        3. Skills
            1. **Super Block**—Build a block with special color, and end the round.
        4. Cool down: 1 round
    3. Engineer (Madeline)
       
        ![Untitled](Design%20Doc/Untitled%202.jpeg)
        
        1. Description: Madeline is an engineer with exceptional engineering skills, known for her swift building speed and excellent hammering skills. Whether she is building massive structures or delicate machinery, she always handles them with ease and accomplishes impressive feats of engineering in a short amount of time. Her yellow attire shines like a dazzling sun, brightening the future of every project. The Engineer has better tools and stronger building abilities. She can build an extra piece of block on her round.
        2. Hit range: 4
        3. Skills
            1. ****Fast Build****—Build 3 random blocks and end your round.
        4. Cool down: 4 round
    4. A Pink Ball (Definitely not Kirby)
       
        ![Untitled](Design%20Doc/Untitled%203.jpeg)
        
        1. Description: It is a little pink ball that ran out from next door. When it confidently tells everyone: "I can stuff everything in my mouth! And also...spit it out!”
        2. Hit range: 4
        3. Skills
            1. **Gobble**—Take one of opponent's block in it mouth and wait until the time to spit it out.
        4. Cool down: 3 round

---

## The Game World

### Overview

- This is a story that takes place in Gehenna. In the historical records of that parallel world, Gehenna has always been born with miracles. Long ago, the kings of various races sent their strongest warriors here, and the champion would bring God's favor to his people. From then on, Gehenna is considered a place where people have achieved great success. And now, you and your enemies have come here at the same time.
You are the most powerful warrior of your race, and you hope to bring glory and wealth to your people through your strength. However, God will not choose two people at the same time. He hopes to bestow favor on the warrior closest to him. The competition is about to start, you need to keep climbing upwards to get this opportunity.
Either elevate yourself or destroy the hope of your enemies.
Do you want to be an unknown person or make a name for yourself?
Unfortunately, your enemy also thinks the same…
- The world of the game features a relaxed, pleasant atmosphere with confrontational and social attributes, and the graphics use pixel art with a predominantly bright color palette as the game's art style.

### Key Locations

- At the beginning: Each player has 12 randomly generated blocks as their initial capital. The followed operations are based on these 12 blocks for the next step.
- When players have only one block on the first time: Players can now have a chance to quickly build two blocks. This can effectively balance the disadvantages caused by previous players' own negligence and objective luck, giving players more opportunities to gain advantages. At the same time, both players have a *Last Stand* opportunity, which allows players to carefully consider how to proceed with their next actions, especially when there are not many blocks left for both players.

### Scale

- **B&H** use a direct operation method. From a dimensional perspective, players only need to consider building their own tower and hitting opponents' tower. However, due to the addition of character skills, randomly gained skills, unpredictable color of the next blocks, and the opportunity of Last Stand, players need to consider more than simple Build and Hit operations in One-dimensional space.
- At the same time, the process of the game is unpredictable. We do not have any pre designed scripts or predictable block colors, and even the probability of skill acquisition is dynamically changing. In this game, when players win, what strategies the two players will use to gain an advantage, or what exciting scenes the game will have are unknown to every game and every player. We leave all the choices in the game to the players themselves. From this perspective, the game itself is open and free.

---

## Camera

### Overview

- As a flat 2D game, the position of the characters and blocks in the world is almost fixed, so we wanted to have other ways to enhance the visual feedback of the game screen to the player. We want to be able to tell the players what is happening right now and what they should be concentrating on by focusing and moving the camera between the various subjects of the game.

### Move

- When turns are exchanged, there is no doubt that we want the character of the current turn to have more weight on the screen, closer to the center of the screen, so each turn exchange returns the character of the current turn to the center of the screen.
- When using a Hit, we expand the entire field of view of the camera to ensure that the player can better judge how to use the Hit and find the target they want to destroy, and when the Hit is triggered, we move the camera to the position where the block will be destroyed.
- When the victory condition is triggered, we will zoom in on the winner and move the loser out of the video frame to tell players who has won the final glory.

### Shake

- We really wanted to give the core element of the game, the combo, its own unique feedback, so we added our favorite camera shake to it, so that when the combo is triggered, the camera gives the player a feedback that catches the eye and creates tension and excitement.

---

## User Interface & Art

### Overview

- **Disclaimer: Most of our art comes from our developer *Zhang Yiyao*. Any similarities are purely coincidental.**
- As a 2D pixel game, we have implemented our minimalistic and cute game art style, from the background of the screen, to the game characters, to all the other elements that appear in the game. We want players to understand and enjoy our art!

### Main Menu

![Resources.png](Design%20Doc/Resources.png)

The main menu is a visual representation of our art style.

The game title Build And Hit shows the two most basic actions of this game: Build, Hit.

The background shows some of the other elements of the game, such as characters and blocks. And we hope to use the elements like clouds, sun and trees to create a happy atmosphere.

### Battle Background(float)

![Background1.png](Design%20Doc/Background1.png)

![Background2_2.png](Design%20Doc/Background2_2.png)

The first picture has a very visual B & H built with bricks. The second picture shows our cute little water monster, appearing as a mascot. And, they are all dynamic.

### Block

![Untitled](Design%20Doc/Untitled%209.png)

We now have four types of blocks for the time being, with the red, green and blue colors being the basic blocks, and the other blue block being a special block exclusive to Slimes. The picture also shows darker versions of the various blocks in the isSelected state, and when Hit selects them, they flash alternately in both light and dark states.

### Character

- Every character contain two basic states, one state of being on standby and the other state is holding up a block during his/her/its turn.
- Dasher
  
    ![Untitled](Design%20Doc/Untitled%2010.png)
    
- Engineer
  
    ![Untitled](Design%20Doc/Untitled%2011.png)
    
- Slime
  
    ![Untitled](Design%20Doc/Untitled%2012.png)
    
    Because slimes are always so cute and round and bouncy, we've created lots of pictures of them to make sure they're always as cute as they look.
    
- A Pink Ball
  
    ![Untitled](Design%20Doc/Untitled%2013.png)
    

The A Pink Ball has a unique state when using its skill, so we added a skill animation to it: this involved more image creation.

### Actions

![Untitled](Design%20Doc/Untitled%2014.png)

Basic two-player action keys and icons are included, as well as cool-down circle.

### Icons & Other Titles

![Untitled](Design%20Doc/Untitled%2015.png)

These contain all the icons and text for the main and pause menus, as well as the banners that appear when splitting win and loss.

- Tutorial
  
    ![TutorialBackground.png](Design%20Doc/TutorialBackground.png)
    

This is a tutorial on the keystrokes players can pull up while paused.

## Music & Sound Effects

### Overview

- The music for this game mainly comes from an album called ***The Ecstasy of Life***. The author of the album, **DDRKirby(ISQ)**, is an original music composer and also an indie game developer. This author used a modern chip sound called “9bit”, conveying the author's philosophy: Don't stop to think; just let the melodies soar and flow.

### Details

- Because **B&H** adopts a pixel-art style, we considered using retro-style 8bit sound track (or 8bit-like style electronic music) as the theme music style. The game's relaxed but confrontational style of play, combined with the player's feel for Build, Hit and even Combo in the game, made us to further consider background music with a rhythmic and optimistic style. In the end, we used the music album of composer DDRKirby(lSQ) ***The Ecstasy of Life.***

### Music Tracks

1. Infinity — DDRKirby(lSQ)
2. Twilight Melody — DDRKirby(lSQ)

Note: These two music tracks in **B&H** have been licensed for non-commercial use by the authors. Author’s website: [https://ddrkirbyisq.bandcamp.com/](https://ddrkirbyisq.bandcamp.com/)

### Sound Effects

- This game's sound effects use royalty-free audio sound effects from ***Adobe Audition*** sound library. Build, Combo, and Hit sound effects in ***Build and Hit*** all come from Audition's sound files.

---

## Multiplayer Game

- For everyone who plays this game, it is not pre-designed bosses and monsters they have to face; the player needs to defeat another player who is just as smart, cunning, strategic and skillful as they are. This is what makes multiplayer game free and open, and what makes this game so much fun due to the multiplayer game.
There are no limits to the difficulty of B&H, and B&H makes no claims about breaking in or level design. It's all up to the player. We constructed the background and art style, set up the rules and created a tense but joyful environment. And then, we leave all the rest to the two players. The two players on the field will give everyone a tense and exciting performance.

---

## Resources & Links

- References and source of inspiration

[Zuma Deluxe](https://store.steampowered.com/app/3330/Zuma_Deluxe/)

[ZUMA FREE GAMES - play Zuma Deluxe, Revenge, Bubbles free online](https://www.zumafreegames.com/)

- Licensed sound tracks

[DDRKirby(ISQ)](https://ddrkirbyisq.bandcamp.com/)

[Adobe Audition Sound Effects](https://www.adobe.com/products/audition/offers/AdobeAuditionDLCSFX.html)

---

## Reference

[DesignDocTemplate.doc](Design%20Doc/DesignDocTemplate.doc)

[Our Design Doc Must has… (1)](https://www.notion.so/Our-Design-Doc-Must-has-1-ab7f5c92d8a449f7b2c429bc6fc4ad8a?pvs=21)