
open System
open System.Windows.Forms
open SeatLayout
open TicketManagement

let bookSeat (row: int) (col: int) (customerName: string) (showtime: string) (layout: (string * bool)[,]) (buttons: Button[,]) =
    if SeatLayout.reserveSeat layout (row, col) then
        let ticketId = TicketManagement.generateTicketId()
        TicketManagement.saveTicketDetails ticketId (layout.[row, col] |> fst) showtime customerName
        SeatLayout.updateSeatButtons buttons layout
        printfn "Seat %s reserved for %s at %s" (layout.[row, col] |> fst) customerName showtime
    else
        printfn "Seat %s is already reserved." (layout.[row, col] |> fst)

let createBookingForm () =
    let form = new Form(Text = "Cinema Booking", Size = Size(800, 600))
    let buttons = Array2D.init 5 5 (fun row col -> new Button(Text = sprintf "R%dC%d" (row+1) (col+1), Size = Size(60, 60), Top = row * 70, Left = col * 70))
    
    for row in 0 .. 4 do
        for col in 0 .. 4 do
            form.Controls.Add(buttons.[row, col])
    
    form
