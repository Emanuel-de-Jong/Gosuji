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

        init(boardsize, handicap, sgfContent, stoneVolume) {
            if (this.placeStoneAudios == null) {
                this.placeStoneAudios = [];

                let fileDir = "resources/audio/pages/trainer/";
                for (let i = 0; i < 5; i++) {
                    let audio = new Audio(fileDir + "placeStone" + i + ".mp3");
                    this.placeStoneAudios.push(audio);
                }
            }

            if (stoneVolume != null) {
                this.setStoneVolume(stoneVolume);
            }

            this.boardsize = boardsize;
            this.setHandicap(handicap);

            this.element = document.querySelector(".board");

            this.besogoOptions.size = this.boardsize;

            delete this.besogoOptions.sgf;
            if (sgfContent) {
                this.besogoOptions.sgf = sgfContent;
            } else if (this.handicap != 0) {
                this.besogoOptions.sgf = "(;" +
                    "SZ[" + this.boardsize + "]" +
                    "HA[" + this.handicap + "]" +
                    "AB" + this.HANDICAP_SGFS[this.boardsize][this.handicap] +
                    ")";
            }

            besogo.create(this.element, this.besogoOptions);

            const stack = [this.element];
            while (stack.length > 0) {
                const currentElement = stack.pop();
                currentElement.classList.add("bsg");
                stack.push(...currentElement.children);
            }

            this.editor = this.element.besogoEditor;

            this.lastMove = this.editor.getCurrent();
        }


        redraw() {
            this.editor.notifyListeners({ markupChange: true });
        }

        setStoneVolume(volume) {
            this.placeStoneAudios.forEach((audio) => audio.volume = volume);
        }

        playPlaceStoneAudio() {
            let placeStoneAudioIndex;
            do {
                placeStoneAudioIndex = utils.randomInt(5);
            } while (this.lastPlaceStoneAudioIndex != null && placeStoneAudioIndex == this.lastPlaceStoneAudioIndex);
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

        addMarkup(x, y, markup) {
            this.editor.getCurrent().markup[(x - 1) * 19 + (y - 1)] = markup;
        }

        removeMarkup(coord) {
            let markup = this.editor.getCurrent().markup;
            markup[(coord.x - 1) * this.boardsize + (coord.y - 1)] = 0;
        }

        clearMarkups() {
            let markup = this.editor.getCurrent().markup;
            for (let i = 0; i < markup.length; i++) {
                if (markup[i] && markup[i] != 4) {
                    markup[i] = 0;
                }
            }
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

        addGhostStone(x, y, color) {
            let colorClass = "besogo-svg-";
            if (color == g.COLOR_TYPE.B) {
                colorClass += "blackStone";
            } else {
                colorClass += "whiteStone";
            }

            let stoneElement = besogo.svgEl("circle", {
                cx: besogo.svgPos(x),
                cy: besogo.svgPos(y),
                r: 42,
                'class': colorClass
            });
            stoneElement.classList.add("ghostStone");
        
            let svgElement = document.querySelector(".besogo-board svg");
            svgElement.appendChild(stoneElement);
        }

        removeGhostStones() {
            document.querySelectorAll(".besogo-board .ghostStone").forEach(gs => gs.remove());
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
