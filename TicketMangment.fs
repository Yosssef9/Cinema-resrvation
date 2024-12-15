
open System
open System.IO
open System.Text.RegularExpressions

let generateTicketId () = Guid.NewGuid().ToString()

let ticketsFilePath = @"C:\CinemaApp\tickets.txt"
Directory.CreateDirectory(Path.GetDirectoryName(ticketsFilePath)) |> ignore

let saveTicketDetails (ticketId: string) (seatId: string) (showtime: string) (customerName: string) =
    let ticketDetails = sprintf "Ticket ID: %s\nSeat: %s\nShowtime: %s\nCustomer: %s\n\n" ticketId seatId showtime customerName
    File.AppendAllText(ticketsFilePath, ticketDetails)
    printfn "Ticket saved: %s" ticketId

let nameRegex = @"^[\p{L}\s]+$"
let showtimeRegex = @"^\d{4}-\d{1,2}-\d{1,2} \d{2}:\d{2}$"

let isValidName (name: string) =
    Regex.IsMatch(name, nameRegex)

let isValidShowtime (showtime: string) =
    Regex.IsMatch(showtime, showtimeRegex)
