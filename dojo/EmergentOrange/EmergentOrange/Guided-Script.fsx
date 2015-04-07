(*
Introduction
++++++++++++++++++++++++++++++++++++++++++++++++++++++

The goal of this dojo is to verify (or disprove) the 
"Emergent Orange" effect described in this blog post:
http://krazydad.com/blog/2013/12/05/emergent-orange/

In a nutshell, "Emergent Orange" states that if you
pull random photographs from the web and average them
together, the resulting image will turn out rusty
orange. 

Here is how we could verify that:
1. Use Flickr to find a sample of random pictures,
2. Download them,
3. Create a composite image, where each pixel will be
the average RGB encoding.

The script below guides you through one possible way
to do that, step by step. It's broken in sections 
0, 1, ..., each explaining the goal, providing hints
on what tools/syntax to use, with a TODO section that
describes, well, what to do :)

Have fun, and feel free to be a rebel and ignore all 
the guidance!
*)


(*
0. Warmup: downloading a picture from the web
++++++++++++++++++++++++++++++++++++++++++++++++++++++

Part of the experiment involves downloading a sample
of images from the internet. To do that, we'll need
to fetch an image from a url, and save it locally. As
a warm-up, let's do that using .NET libraries.
*)

// TUTORIAL: DOWNLOADING A FILE 
// This points to the F# Software Foundation logo:
// http://fsharp.org/img/logo.png

let logoUrl = """http://fsharp.org/img/logo.png"""

open System.Net
open System.Drawing

let downloadLogo () =   
    let request = logoUrl |> WebRequest.Create
    use response = request.GetResponse ()
    use stream = response.GetResponseStream ()
    let image = stream |> Image.FromStream
    let targetLocation = __SOURCE_DIRECTORY__ + "/logo.png"    
    image.Save (targetLocation)

// To run this, simply call
// downloadLogo ()
// This should download that file into the folder 
// where this script is located.

// END TUTORIAL


// TODO: write a downloadImage function that takes in
// any url and downloads the image.

let downloadImage =
    // YOUR CODE GOES HERE
    printfn "Implement me now!"


(*
1. Finding random pictures from Flickr
++++++++++++++++++++++++++++++++++++++++++++++++++++++ 

We will use the XML type provider to simplify calling
the Flickr API. We'll start with an illustration of 
how the Type Provider works, then it's your job to 
make it work with Flick!
*)

// TUTORIAL: XML TYPE PROVIDER 
// Check the following page, which describes how to 
// call the OpenWeather API, and ask for weather
// data for particular locations: 
// http://openweathermap.org/current

// For instance, this is how you would ask for the 
// weather in London. If you hit that url, you will
// get back an XML response:
// http://api.openweathermap.org/data/2.5/weather?q=London&mode=xml

// Let's use the XML type provider to automatically
// create a CityWeather type, based on the XML sample:

#r @"..\packages\FSharp.Data.2.2.0\lib\net40\FSharp.Data.dll"
open FSharp.Data
#r @"System.Xml.Linq.dll"
open System.Xml.Linq

type CityWeather = XmlProvider<"""http://api.openweathermap.org/data/2.5/weather?q=London&mode=xml""">
let london = CityWeather.GetSample ()

// try typing london. in this file to explore the 
// result you got back...

printfn "Minimum temp in %s: %f %s" london.City.Name london.Temperature.Min london.Temperature.Unit

// you can now use the same type to make calls again:
let weatherIn (cityName:string) =
    let requestUrl = sprintf "http://api.openweathermap.org/data/2.5/weather?q=%s&mode=xml" cityName
    requestUrl
    |> CityWeather.Load

// Note: the JSON type provider follows exactly the
// same pattern.

// END TUTORIAL


// Your turn now. Your task is to use the flickr API 
// to create a sample of random images, identified by
// their url. In the next phase, we will use the urls
// to download images on your local machine.
// You probably want at least 10 images, ideally as
// different as possible.

// Flickr API documentation:
// https://www.flickr.com/services/api/

// The following call searches photos by search term,
// and could be used to create a random sample:
// https://www.flickr.com/services/api/flickr.photos.search.html

// This page explains how the url to a photo works:
// https://www.flickr.com/services/api/misc.urls.html

// You need a Flickr API key to call the API; you can 
// create a free account for this.

// Flickr API key
let apiKey = "99bc6e017593e1d5f41f6e0d09cb7286"

// Utility function: finds the URL of a Flickr picture
let flickrPhotoUrl (farm,server,photoID:int64,secret) =
    sprintf "https://farm%i.staticflickr.com/%i/%i_%s.jpg" farm server photoID secret   

let example = flickrPhotoUrl (8,7652,16882873966L,"06bebe6c57")    

// YOUR CODE GOES HERE

// TODO: write a function, that, given a search term,
// returns a list of image and data from Flickr:

let findPictures (searchTerm:string) =
    printfn "Implement me now!"


(*
2. Creating a sample
++++++++++++++++++++++++++++++++++++++++++++++++++++++ 

We know how to download an image, we know how to get
image urls from Flickr - time to create a sample. You
could for instance take 5 very different words, search
Flickr for these, download them, and save them using
the code from part 0.
*)


// TODO: create a sampe of random images from Flickr


(*
3. Merging images together
++++++++++++++++++++++++++++++++++++++++++++++++++++++ 

Now that we have a couple of images, let's merge them!
Two things need to happen for that:
- the images should be on a consistent size,
- we need to figure out how to create a composite.
For the composite, we will simply take images from our
sample, and for every pixel, we'll compute the 
average RGB (red/green/blue) value across the sample.
*)

// Here are a couple of useful functions / demos:

// take an image and resize it:
let resize (img:Image) = new Bitmap(img, Size(100,100)) :> Image
// take an image and save it
let saveAs (where:string) (img:Image) = img.Save where
// get a pixel color
let pixelColor (bitmap:Bitmap) (col,row) =
    bitmap.GetPixel(col,row)

// Colors and RGB values
let testColor = Color.Goldenrod
printfn "R:%i G:%i B:%i" testColor.R testColor.G testColor.B

let myOrange = Color.FromArgb(255,128,0)


// TODO: compute a composite image from the sample 


(*
4. Epilogue
++++++++++++++++++++++++++++++++++++++++++++++++++++++ 

Does it work? Is your composite rust-orange-ish?

If you had fun, feel free to tweet your result, with
the hashtag #fsharp; and mentions of @c4fsharp are
always appreciated :)


Now that you have this working, you could try to make
the code better (for instance). Two directions:

- make the image download asynchronous:
http://fsharpforfunandprofit.com/posts/concurrency-async-and-parallel/

- use a bit of parallelism to compute the composite?

*)