class TrainerBoard extends Board {
    constructor() {
        super();

        this.besogoOptions.panels = "control+names+tree+comment+file";
    }

    init(serverBoardsize, serverHandicap, serverSGF) {
        this.handicapElement = document.getElementById("currentHandicap");

        super.init(serverBoardsize, serverHandicap, serverSGF);
    
        // Disable mouse 3/4 triggering prev/next in browser
        document.addEventListener("mouseup", (e) => {
            if (typeof e === "object" && (e.button == 3 || e.button == 4)) {
                e.preventDefault();
            }
        });
        utils.addEventsListener(document, ["keydown", "mousedown"], this.keydownAndMousedownListener);
    }

    clear(serverBoardsize, serverHandicap, serverSGF) {
        super.clear(serverBoardsize ? serverBoardsize : settings.boardsize,
            serverHandicap != null ? serverHandicap : settings.handicap,
            serverSGF);
    
        document.querySelector('#game button[title="Variants: [child]/sibling"]').remove();
        document.querySelector('#game button[title="Variants: show/[hide]"]').remove();
        document.querySelector('#game input[value="9x9"]').remove();
        document.querySelector('#game input[value="13x13"]').remove();
        document.querySelector('#game input[value="19x19"]').remove();
        document.querySelector('#game input[value="?x?"]').remove();
        document.querySelector('#game input[value="Comment"]').remove();
        document.querySelector('#game input[value="Edit Info"]').remove();
        document.querySelector('#game input[value="Info"]').remove();
    
        document
            .querySelector("#game .besogo-board")
            .insertAdjacentHTML(
                "beforeend",
                '<button type="button" class="btn btn-secondary next" disabled>></button>'
            );
        this.nextButton = document.querySelector(".next");
    
        this.editor.addListener(gameplay.playerMarkupPlacedCheckListener);
        this.editor.addListener(gameplay.treeJumpedCheckListener);
        this.editor.addListener(sgf.boardEditorListener);
        this.nextButton.addEventListener("click", gameplay.nextButtonClickListener);
        G.phaseChangedEvent.add(this.phaseChangedListener);
    
        // console.log(besogo);
        // console.log(this.editor);
        // console.log(this.editor.getCurrent());
    }

    async play(suggestion, moveType = G.MOVE_TYPE.NONE, tool = "auto") {
        await this.draw(suggestion.coord, tool, true, moveType);
        scoreChart.update(suggestion);
        sgfComment.setComment(moveType);
    }

    async draw(coord, tool = "auto", sendToServer = true) {
        this.editor.setTool(tool);
        this.editor.click(coord.x, coord.y, false, false);
        this.editor.setTool("navOnly");
    
        if (tool == "auto" || tool == "playB" || tool == "playW") {
            if (G.phase == G.PHASE_TYPE.CORNERS || G.phase == G.PHASE_TYPE.PREMOVES || G.phase == G.PHASE_TYPE.GAMEPLAY) {
                this.playPlaceStoneAudio();
            }
    
            this.lastMove = this.editor.getCurrent();
    
            G.suggestionsHistory.add(G.suggestions);
    
            if (sendToServer) {
                if (tool == "auto") {
                    await katago.play(coord);
                } else if (tool == "playB") {
                    await katago.play(coord, G.COLOR_TYPE.B);
                } else if (tool == "playW") {
                    await katago.play(coord, G.COLOR_TYPE.W);
                }
            }
        }
    }

    async syncWithServer() {
        await katago.clearBoard();
        await katago.setHandicap();
        await katago.playRange();
    }

    drawCoords(suggestionList) {
        let suggestions = suggestionList.getFilterByWeaker();
    
        let markup = this.editor.getCurrent().markup;
        for (let i = 0; i < markup.length; i++) {
            if (markup[i] && markup[i] != 4) {
                markup[i] = 0;
            }
        }
    
        this.editor.setTool("label");
        for (let i = 0; i < suggestions.length; i++) {
            let coord = suggestions[i].coord;
    
            this.editor.setLabel(suggestions[i].grade);
            this.editor.click(coord.x, coord.y, false, false);
        }
    
        this.editor.setTool("navOnly");
    }

    setHandicap(handicap) {
        super.setHandicap(handicap);

        this.handicapElement.innerHTML = handicap;
        if (G.phase != G.PHASE_TYPE.NONE && G.phase != G.PHASE_TYPE.INIT) sgf.setHandicapMeta();
    }

    keydownAndMousedownListener = (event) => {
        if (event.code == "Space" || event.code == "Enter" || event.button == 1 || event.button == 3 || event.button == 4) {
            this.nextButton.click();
        }
    }

    phaseChangedListener = (e) => {
        if (e.phase == G.PHASE_TYPE.GAMEPLAY || e.phase == G.PHASE_TYPE.FINISHED) {
            this.editor.setIsTreeJumpAllowed(true);
        } else {
            this.editor.setIsTreeJumpAllowed(false);
        }
    }
}
