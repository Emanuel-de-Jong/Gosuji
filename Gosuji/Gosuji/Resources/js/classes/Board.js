let Board = window.Board;

if (typeof Board === "undefined") {
    Board = class Board {
        HANDICAP_SGFS = {
            19: {
                2: "[pd][dp]",
                3: "[dd][pd][dp]",
                4: "[dd][pd][dp][pp]",
                5: "[dd][pd][jj][dp][pp]",
                6: "[dd][pd][dj][pj][dp][pp]",
                7: "[dd][pd][dj][jj][pj][dp][pp]",
                8: "[dd][jd][pd][dj][pj][dp][jp][pp]",
                9: "[dd][jd][pd][dj][jj][pj][dp][jp][pp]",
            },
            13: {
                2: "[jd][dj]",
                3: "[dd][jd][dj]",
                4: "[dd][jd][dj][jj]",
                5: "[dd][jd][gg][dj][jj]",
                6: "[dd][jd][dg][jg][dj][jj]",
                7: "[dd][jd][dg][gg][jg][dj][jj]",
                8: "[dd][gd][jd][dg][jg][dj][gj][jj]",
                9: "[dd][gd][jd][dg][gg][jg][dj][gj][jj]",
            },
            9: {
                2: "[gc][cg]",
                3: "[cc][gc][cg]",
                4: "[cc][gc][cg][gg]",
                5: "[cc][gc][ee][cg][gg]",
                6: "[cc][gc][ce][ge][cg][gg]",
                7: "[cc][gc][ce][ee][ge][cg][gg]",
                8: "[cc][ec][gc][ce][ge][cg][eg][gg]",
                9: "[cc][ec][gc][ce][ee][ge][cg][eg][gg]",
            },
        };

        besogoOptions = {
            resize: "auto",
            orient: "portrait",
            coord: "western",
            tool: "navOnly",
            variants: 2,
            nowheel: true,
        };

        init(boardsize, handicap, sgf) {
            let fileDir = "resources/audio/pages/trainer/";
            this.placeStoneAudios = [
                new Audio(fileDir + "placeStone0.mp3"),
                new Audio(fileDir + "placeStone1.mp3"),
                new Audio(fileDir + "placeStone2.mp3"),
                new Audio(fileDir + "placeStone3.mp3"),
                new Audio(fileDir + "placeStone4.mp3"),
            ];
            this.lastPlaceStoneAudioIndex = 0;

            this.clear(boardsize, handicap, sgf);
        }

        clear(boardsize, handicap, sgf) {
            this.boardsize = boardsize;
            this.setHandicap(handicap);

            this.element = document.querySelector(".board");

            this.besogoOptions.size = this.boardsize;

            delete this.besogoOptions.sgf;
            if (sgf) {
                this.besogoOptions.sgf = sgf;
            } else if (this.handicap != 0) {
                this.besogoOptions.sgf = "(;" +
                    "SZ[" + this.boardsize + "]" +
                    "HA[" + this.handicap + "]" +
                    "AB" + this.HANDICAP_SGFS[this.boardsize][this.handicap] +
                    ")";
            }

            besogo.create(this.element, this.besogoOptions);

            this.editor = this.element.besogoEditor;

            this.commentElement = document.querySelector("#game .besogo-comment textarea");

            this.lastMove = this.editor.getCurrent();
        }

        playPlaceStoneAudio() {
            let placeStoneAudioIndex;
            do {
                placeStoneAudioIndex = utils.randomInt(5);
            } while (placeStoneAudioIndex == this.lastPlaceStoneAudioIndex);
            this.lastPlaceStoneAudioIndex = placeStoneAudioIndex;

            this.placeStoneAudios[placeStoneAudioIndex].play();
        }

        getColor() {
            let currentMove = this.editor.getCurrent();
            if (currentMove.move) {
                return currentMove.move.color;
            }

            if (currentMove.moveNumber == 0) {
                return !this.handicap ? g.COLOR_TYPE.W : g.COLOR_TYPE.B;
            }

            return g.COLOR_TYPE.B;
        }

        getNextColor() {
            let currentMove = this.editor.getCurrent();
            if (currentMove.children && currentMove.children.length > 0) {
                return currentMove.children[0].move.color;
            }

            return this.getColor() * -1;
        }

        findStone(coord) {
            return this.editor.getCurrent().getStone(coord.x, coord.y);
        }

        pass() {
            this.editor.click(0, 0, false);
        }

        removeMarkup(coord) {
            let markup = this.editor.getCurrent().markup;
            markup[(coord.x - 1) * this.boardsize + (coord.y - 1)] = 0;
        }

        goToNode(nodeNumber) {
            let currentNodeNumber = this.getMoveNumber();
            let nodesToJump = nodeNumber - currentNodeNumber;
            if (nodesToJump > 0) {
                this.editor.nextNode(nodesToJump);
            } else if (nodesToJump < 0) {
                this.editor.prevNode(nodesToJump * -1);
            }
        }

        getMoveNumber() {
            return this.editor.getCurrent().moveNumber;
        }

        getNodeX() {
            return this.editor.getCurrent().navTreeX;
        }

        getNodeY() {
            return this.editor.getCurrent().navTreeY;
        }

        getNodeCoord() {
            return new Coord(this.getNodeX(), this.getNodeY());
        }

        setHandicap(handicap) {
            this.handicap = handicap;
        }
    }

    window.Board = Board;
}

export { Board };
