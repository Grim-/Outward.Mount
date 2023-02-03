If you are familiar with Unity and the AssetBundle process, the mount mod was made from the start with the intent of people being able to if they wish create and add their own mount models to the mod itself.


I will include a brief overview, if you wish to learn more you can find me in the [Outward Modding Discord](https://discord.gg/BBfxSD7M) I go by the name Emo.

 
For this example I will cover the structure of the Manticore prefab.

At it's core the Mount mod relies on the root of the prefab having a CharacterController component on it and a correctly set up Animator component.


![The Manticore Prefab](https://user-images.githubusercontent.com/3288858/216479457-8099dcad-7c57-48dd-887e-ce6d416e0fd0.png)


Aside from this each mount will need atleast one empty gameobject named exactly "SL_MOUNTPOINT" correctly created and parented where ever you wish the player to be attached to when they mount up, generally you want this somewhere in the armature itself.

Note in the picture below the co-ordinate space is local and not global (red box) this means when you click the SL_MOUNTPOINT the players up direction will be the same as the green arrow and it's forward direction the same as the blue arrow.

![Mount Points](https://user-images.githubusercontent.com/3288858/216479966-1c793f1b-d6e2-46de-9a67-3cd73137afb1.png)

