Hudson Hughes, hlhughes@uci.edu, hlhughes, 12712020

Play this at 1024 x 768

This is basically Connect 4 with a minimax/alpha-beta pruning AI.

Basic Introduction: Tribal Rivals is a turn based board game where players compete to be the first to arrange pieces into a win state. Both players, the actual human and the AI, take turns dropping red or blue pieces into columns on a 7x6 board. They are both trying to get four of their pieces to create 4 piece long lines, while obstructing the other players lines. Once per game each player can remove all the pieces in a column and rotate a full row of pieces by one. The narrative context is that there is a rival tribe attempting to destroy yours by constructing a monument to summon a volcano god. You must build your own monuments in the same area to keep the volcano god appeased. 

The Target Experience: The experience I try to give players of Tribal Rivals is the thrill of slow strategic deliberation, similar to what one gets playing a turn based game. Of course, it might be easy to conquer a minimax algorithm and remember the order of moves it takes to win. The implementation of a basic learning component creates emphasizes the experience even through extensive play.

Mechanics:
Drop Piece Ability
Delete Column Ability
Shift Row Ability
Minimax AI that takes those moves into account
Turn Based Strategy
Opponent that learns
