using System;
using System.Reflection;

public class system{
    public static bool iterate {get;}=true;

    //just for testing, echo the inputt string
    public static void echo(string input, TIM.main game){
        Console.WriteLine(input);
    }

    //create a completely new map
    public static void newMap(TIM.main game){
        game.entities = new Dictionary<string,functionProperties>();
        game.gameMap.createMap();
        game.entities.Add("sys",new functionProperties());
    }

    //print the map pixel's properties in text form
    public static void printMap(TIM.main game){
        game.gameMap.print();
    }

    //display the map 
    public void displayMap(TIM.main game){
        game.gameMap.display(game.center_pos.X-game.zoom*25,game.center_pos.Y-game.zoom*25,game.zoom*50,game.zoom*50,game.zoom,game);
    }

    //create a new game Entity
    public void newEntity(string input,TIM.main game){
        string[] inp = input.Split(",");
        try{
            new functionProperties(inp[0]);
        }catch(NotImplementedException){
            return;
        }
        functionProperties entity=new functionProperties(inp[0]);
        var boolVar= entity.fType.GetProperty("createByPlayer");
        if(boolVar!=null){  //if the entity could be created by the player directly
            var boolVal= boolVar.GetValue(entity.fObject, null);
            bool creatable = (bool)boolVal;
            if(creatable==true){    //if the entity can be created by the player directly
                try{
                    functionProperties props = new functionProperties(inp[0]);  //instanciate the entity
                    game.entities.Add(inp[1],props);    //add the entity to the Dictionary of entities
                    if(inp.Length>2){   //if arguments are given
                        string args="";
                        //parse the arguments
                        for(int i=2; i<inp.Length; i++){
                            args+=inp[i]+",";
                        }
                        try{
                            methods.callMethod(inp[1],"setup",args,game);   //try to call the setup method
                            this.displayMap(game);
                        }catch(TargetInvocationException ex){
                            if(ex.InnerException is NotSupportedException){
                                game.entities.Remove(inp[1]);
                                Console.WriteLine(ex.InnerException.Message);
                            }
                        }   
                    }else{  //if no arguments are given
                        try{
                            methods.callMethod(inp[1],"setup",game);    //try to call the setup method
                            this.displayMap(game);
                        }catch(TargetInvocationException ex){
                            if(ex.InnerException is NotSupportedException){
                                Console.WriteLine("INF: No setup function for {0} found",inp[0]);
                            }
                        } 
                    }
                }catch (ArgumentException){
                    Console.WriteLine("INF: Entity '{0}' already exists.",inp[1]);  //cath the name allready being taken
                }
            }else{
                Console.WriteLine("INF: Entity of Class '{0}' cannot be created by player.",inp[0]); 
            }
        }else{
            Console.WriteLine("INF: Entity of Class '{0}' cannot be created by player.",inp[0]);
        }
    }

    //shorthand for newEntity()
    public void nE(string input, TIM.main game){
        this.newEntity(input,game);
    }

    //quit the game
    public static void exit(TIM.main game){
        ui.printOutro();
        Console.ReadKey();
        game.exit=true;
    }

    //set Zoomlevel for map drawing
    public static void setZoom(string input, TIM.main game){
        game.zoom=int.Parse(input);
    }

    //set center position for map drawing
    public static void setPosition(string input, TIM.main game){
        int x=int.Parse(input.Split(",")[0]);
        int y=int.Parse(input.Split(",")[1]);
        game.center_pos.X = x;
        game.center_pos.Y=y;
    }

    //list all game entities
    public void listEntities(TIM.main game){
        foreach(KeyValuePair<string, functionProperties> entry in game.entities){
            Console.WriteLine("{0}: {1} , {2}",entry.Key,entry.Value.fType);
        }
    }

    //universal step function which is executed every frame
    public void step(TIM.main game){
        this.displayMap(game);
    }
}