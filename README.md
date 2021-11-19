# Bit Calculator
This asset allows users to convert literal strings into a bit output, and allows for easy bit flipping in-editor.

## Usage
Simply head to the top of Unity, and open the window from `Tools/Nomnom/Bit Calculator`

## Installation
#### Using Unity Package Manager
1. Open the Package Manager from `Window/Package Manager`
2. Click the '+' button in the top-left of the window
3. Click 'Add package from git URL'
4. Provide the URL of this git repository: https://github.com/nomnomab/BitCalculator.git
5. Click the 'add' button

![Overview](./GitResources~/overview.png)

## What Can It Do?
1. Has an input field that can take a literal string for a bit "formula"
2. Bits are shown at the bottom of the window. 
   - These can be pressed to get "flipped" to their opposite value (0 to 1 and 1 to 0 respectively)
3. Shows the formula output in multiple values:
   - Hexadecimal
   - Decimal
   - Octal
   - Binary

## Supported Operations
> Integer operations only
- `<<` → Left shift
- `>>` → Right shift
- `&` → AND
- `|` → OR
- `^` → XOR
- `~` → Complement (Invert)
- `()` → Groups
  - Single numbers inside of groups are not currently supported `e.g. (5)`
- `+` → Addition
- `-` → Subtraction
- `*` → Multiplication
- `/` → Division
- `**` → Exponent

## Notes
- The light mode version of the window looks a bit... bad. This will be improved at a later time.