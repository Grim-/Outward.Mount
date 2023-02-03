If you are familiar with Unity and the AssetBundle process, the mount mod was made from the start with the intent of people being able to if they wish create and add their own mount models to the mod itself.


I will include a brief overview, if you wish to learn more you can find me in the [Outward Modding Discord](https://discord.gg/BBfxSD7M) I go by the name Emo.

 
For this example I will cover the structure of the Manticore prefab.

At it's core the Mount mod relies on the root of the prefab having a CharacterController component on it and a correctly set up Animator component.


![The Manticore Prefab](https://user-images.githubusercontent.com/3288858/216479457-8099dcad-7c57-48dd-887e-ce6d416e0fd0.png)


Aside from this each mount will need atleast one empty gameobject named exactly "SL_MOUNTPOINT" correctly created and parented where ever you wish the player to be attached to when they mount up, generally you want this somewhere in the armature itself.

Note in the picture below the co-ordinate space is local and not global (red box) this means when you click the SL_MOUNTPOINT the players up direction will be the same as the green arrow and it's forward direction the same as the blue arrow.

![Mount Points](https://user-images.githubusercontent.com/3288858/216479966-1c793f1b-d6e2-46de-9a67-3cd73137afb1.png)



Once you have successfully prepared your model, you will need to set up the Animator Controller. 

Due to a lack of consistent animations across the mount models I have kept the animator controller very simple needing only a Idle, Walk and Run animation to function.

The mod BasicMountController class allows you to also optionally use a Happy, Angry, Sad, On hit and Attack animations.

![image](https://user-images.githubusercontent.com/3288858/216481266-d377a753-a19a-4ce8-a4c9-62a462c28f24.png)


Now your base Animator controller is created you need to set up some parameters in the Animator window, you can do this by double clicking your newly created Animator Controller (or Override Controller if you know what you're doing).

Then pressing the **"+"** in the left hand pane please be sure the parameters tab is selected and not the layers tab.

The only required parameter is :

- Float - "Move Z"

You can optionally also provide trigger parameters named DoMountSpecial, DoMountHitReact, DoMountAttack, DoMountHappy, DoMountAngry and the mod will use these where appropiate.

This "Move Z" values are automatically fed to the animator by the mod allowing the animator to know which animation it should be playing right now.

![image](https://user-images.githubusercontent.com/3288858/216481309-674dd32c-9f7d-425f-b87f-b5f6c3750f5a.png)


Once you have these you can set up the animator component as you normally would in any other Unity project, again I don't intend to cover this indepth but I will show and explain how the current mounts are set up.

This is the Animator window for the Manticore

![image](https://user-images.githubusercontent.com/3288858/216481584-76208bc9-519d-4807-a979-da5b5c374f2e.png)


As you can see the DoMount animations are simply trigger animations that can be transitioned to from *any* state when the condition DoMountAttack is true, the exit condition simply waits until the animation has finished and exits (notice the exit time is ticked for this)

![image](https://user-images.githubusercontent.com/3288858/216481743-b8d9bbf9-7ec3-4f60-9585-b0ac0ba0f914.png)

![image](https://user-images.githubusercontent.com/3288858/216481801-bb520fec-565e-4707-a763-7385bcab80a9.png)




The Movement state itself is actually a Blend tree (you can create these by right clicking in the graph New > From New Blend Tree) the blend trees use the Z value (forward) to determine which animation should be played.


![image](https://user-images.githubusercontent.com/3288858/216481945-5a030a19-065c-4793-8678-45d4f3df7299.png)


As you can see, when the move Z value is 0 the mount is idle, otherwise it will blend from Idle > Walk > Run depending on how quickly your mount model is moving in-game.


 

