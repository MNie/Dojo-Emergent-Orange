open System.Net
open System.Drawing

#r @"../packages/FSharp.Data.2.2.0/lib/net40/FSharp.Data.dll"
open FSharp.Data
#r @"System.Xml.Linq.dll"
open System.Xml.Linq

#r @"../packages/FSharp.Collections.ParallelSeq.1.0.2/lib/net40/FSharp.Collections.ParallelSeq.dll"
open FSharp.Collections.ParallelSeq

let imagesFolderName = @"\images\"
let imagesSource = __SOURCE_DIRECTORY__ + imagesFolderName
let imagesFolder = System.IO.Directory.CreateDirectory imagesSource
let predefinedHeight = 1000
let predefinedWidth = 1000
let numberOfImages = 5

let downloadImage (url: string) =
    let request = url |> WebRequest.Create
    use response = request.GetResponse ()
    use stream = response.GetResponseStream ()
    let targetLocation = imagesSource + (url.Split('/') |> Seq.last)
    stream 
    |> Image.FromStream
    |> fun x -> x.Save (targetLocation)

// Flickr API key
let apiKey = "99bc6e017593e1d5f41f6e0d09cb7286"

let flickrPhotoUrl (farm,server,photoID:int64,secret) =
    sprintf "https://farm%i.staticflickr.com/%i/%i_%s.jpg" farm server photoID secret  

let createFlickrApiUrl searchTerm =
    sprintf "https://api.flickr.com/services/rest/?method=flickr.photos.search&format=json&nojsoncallback=?&api_key=%s&text=%s" apiKey searchTerm

//nojsoncallback in url to get valid json instead of jsonp
type flickrJsonProvider = JsonProvider<"""https://api.flickr.com/services/rest/?method=flickr.photos.search&format=json&nojsoncallback=?&api_key=99bc6e017593e1d5f41f6e0d09cb7286&text=lel""">

let downloadImagesBasedOnSearchTerm searchTerm =
    createFlickrApiUrl searchTerm
    |> flickrJsonProvider.Load
    |> fun x -> x.Photos.Photo
    |> Array.take numberOfImages
    |> Array.Parallel.map (fun x -> downloadImage (flickrPhotoUrl (x.Farm, x.Server, x.Id, x.Secret)))
    |> ignore

let listOfWords = [
    "lel"
    "lul"
    "tak"
    "nie"
    "owszem"
    "polska"
]

let downloadPackageOfImages (listOfWords) =
    listOfWords
    |> PSeq.iter downloadImagesBasedOnSearchTerm

let resize (img:Image) = new Bitmap(img, Size(predefinedWidth, predefinedHeight))

let pixelColor (bitmap:Bitmap) (col,row) =
    bitmap.GetPixel(col,row)

let composite (colors: Color seq) =
    let red = colors |> PSeq.averageBy (fun color -> color.R |> float) |> int
    let green = colors |> PSeq.averageBy (fun color -> color.R |> float) |> int
    let blue = colors |> PSeq.averageBy (fun color -> color.R |> float) |> int
    Color.FromArgb(red, green, blue)

let loadSamples = 
    downloadPackageOfImages (listOfWords)
    imagesFolder.EnumerateFiles () 
    |> PSeq.filter (fun file -> file.Extension = ".jpg")
    |> PSeq.map (fun file -> Bitmap.FromFile(file.FullName))
    |> PSeq.map resize
    |> PSeq.toList

let computeOrange =
    let orange = new Bitmap(predefinedWidth, predefinedHeight)
    for col in 0 .. 999 do
        for row in 0 .. 999 do
            loadSamples
            |> PSeq.map (fun bitmap -> bitmap.GetPixel(col, row))
            |> composite
            |> fun color -> orange.SetPixel(col, row, color)
    orange.Save(imagesSource + "orange.bmp")
