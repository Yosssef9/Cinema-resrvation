
open System.Drawing
open System.Windows.Forms

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
