FBInstant.initializeAsync().then(function () {
    //This is where you load the game assets
    console.log("Loading to 100")
    FBInstant.setLoadingProgress(100)

})

FBInstant.startGameAsync().then(function () {
    console.log("starting !")
    game.start();
    console.log("starting complete!")
})