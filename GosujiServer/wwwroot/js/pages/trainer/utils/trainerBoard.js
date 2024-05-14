var trainerBoard = { ...board };

trainerBoard.ogInit = trainerBoard.init;
trainerBoard.init = function (serverBoardsize, serverHandicap, serverSGF) {
    trainerBoard.ogInit(serverBoardsize, serverHandicap, serverSGF);
};