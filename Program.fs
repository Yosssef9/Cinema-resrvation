open System
open System.Drawing
open System.Windows.Forms
open System.IO
open System.Text.RegularExpressions

let seatLayout: (string * bool)[,] = Array2D.init 5 5 (fun row col -> (sprintf "R%dC%d" (row+1) (col+1), false))

let reserveSeat (layout: (string * bool)[,]) (row, col) =
    let (seatId, isReserved) = layout.[row, col]
    if not isReserved then
        layout.[row, col] <- (seatId, true)
        true 
    else
        false 

let updateSeatButtons (buttons: Button[,]) (layout: (string * bool)[,]) =
    for row in 0 .. Array2D.length1 layout - 1 do
        for col in 0 .. Array2D.length2 layout - 1 do
            let (_, isReserved) = layout.[row, col]
            buttons.[row, col].BackColor <- if isReserved then Color.Red else Color.Green

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

let createForm () =
    let form = new Form(Text = "Cinema Seat Reservation", Width = 600, Height = 700)

    let seatButtons = Array2D.init 5 5 (fun row col ->
        let button = new Button(Text = sprintf "R%dC%d" (row+1) (col+1), Width = 80, Height = 50, Top = 50 * row + 50, Left = 80 * col + 30)
        button.BackColor <- Color.Green
        button
    )

    for row in 0 .. 4 do
        for col in 0 .. 4 do
            form.Controls.Add(seatButtons.[row, col])

    let rowLabel = new Label(Text = "Row Index:", Top = 300, Left = 30)
    let rowBox = new TextBox(Top = 320, Left = 30, Width = 50)

    let colLabel = new Label(Text = "Column Index:", Top = 350, Left = 30)
    let colBox = new TextBox(Top = 370, Left = 30, Width = 50)

    let nameLabel = new Label(Text = "Customer Name:", Top = 400, Left = 30)
    let nameBox = new TextBox(Top = 420, Left = 30, Width = 200)

    let showtimeLabel = new Label(Text = "Showtime :", Top = 450, Left = 30)
    let showtimeBox = new TextBox(Top = 470, Left = 30, Width = 200)

    let confirmButton = new Button(Text = "Confirm Booking", Top = 500, Left = 30, Width = 200)

    let confirmationLabel = new Label(Text = "", Top = 550, Left = 30, Width = 500, Height = 40)
    
    form.Controls.Add(rowLabel)
    form.Controls.Add(rowBox)
    form.Controls.Add(colLabel)
    form.Controls.Add(colBox)
    form.Controls.Add(nameLabel)
    form.Controls.Add(nameBox)
    form.Controls.Add(showtimeLabel)
    form.Controls.Add(showtimeBox)
    form.Controls.Add(confirmButton)
    form.Controls.Add(confirmationLabel)

    confirmButton.Click.Add(fun _ ->
        let rowText = rowBox.Text
        let colText = colBox.Text
        let customerName = nameBox.Text
        let showtime = showtimeBox.Text

        if String.IsNullOrWhiteSpace(rowText) || String.IsNullOrWhiteSpace(colText) || String.IsNullOrWhiteSpace(customerName) || String.IsNullOrWhiteSpace(showtime) then
            MessageBox.Show("Please fill in all fields.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error) |> ignore
        else
            match Int32.TryParse(rowText), Int32.TryParse(colText) with
            | (true, row), (true, col) ->
                if row < 0 || row > 4 || col < 0 || col > 4 then
                    MessageBox.Show("Please enter valid row and column indices (0-4).", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error) |> ignore
                elif not (isValidName customerName) then
                    MessageBox.Show("Please enter a valid name.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error) |> ignore
                elif not (isValidShowtime showtime) then
                    MessageBox.Show("Please enter a valid showtime (YYYY-MM-DD HH:MM).", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error) |> ignore
                else
                    if reserveSeat seatLayout (row, col) then
                        updateSeatButtons seatButtons seatLayout
                        let (seatId, _) = seatLayout.[row, col]
                        let ticketId = generateTicketId()
                        saveTicketDetails ticketId seatId showtime customerName
                        confirmationLabel.Text <- sprintf "Seat %s reserved! Ticket ID: %s" seatId ticketId
                    else
                        confirmationLabel.Text <- sprintf "Seat R%dC%d is already reserved!" (row+1) (col+1)
            | _ -> 
                MessageBox.Show("Please enter valid numeric values for Row and Column.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error) |> ignore
    )

    form

[<STAThread>]
[<EntryPoint>]
let main argv =
    let form = createForm()
    Application.Run(form)
    0
