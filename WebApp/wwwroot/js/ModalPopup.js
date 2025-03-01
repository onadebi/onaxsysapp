class ModalPopup {
    constructor(
        {
            modalBody,
            modalHeader = "",
            modalFooter = "",
            btnOpenModalId = "btnModal",
        } = {},
        {
            modalHeaderBgColor = "",
            modalBodyBgColor = "",
            modalFooterBgColor = "",
            modalWidth = "80%",
            modalTop = "5%",
        } = {}
    ) {
        this.modalHeader = modalHeader;
        this.modalBody = modalBody;
        this.modalFooter = modalFooter;
        this.btnOpenModalId = btnOpenModalId;
        this.initialise();
        this.modalStylesOverride = {
            modalHeaderBgColor: modalHeaderBgColor,
            modalBodyBgColor: modalBodyBgColor,
            modalFooterBgColor: modalFooterBgColor,
            modalWidth: modalWidth,
            modalTop: modalTop,
        };
        this.createStyles();
    }

    initialise = () => {
        this.popupContainer = document.createElement("div");
        this.popupContainer.style.display = "none";

        document.body.appendChild(this.popupContainer);
        this.popupContainer.innerHTML = `
            <div id="dv-onax-Modal" class="onax-modal">
                <div class="onax-modal-Content">
                    <div class="onax-modal-header" style="position:relative;padding:20px;">
                        <span class="onax-closeBtn" style="position:absolute; right:15px;top:8px; font-size:25px;">&times;</span>
                        ${this.modalHeader && this.modalHeader.length > 0
                ? `${this.modalHeader}`
                : ""
            }
                    </div>
                    <div class="onax-modal-body">
                        ${this.modalBody && this.modalBody.length > 0
                ? `${this.modalBody}`
                : ""
            }    
                    </div>
                    <div class="onax-modal-footer">
                        ${this.modalFooter && this.modalFooter.length > 0
                ? `${this.modalFooter}`
                : ""
            } 
                    </div>
                </div>
            </div>
        `;

        const btnClick = document.getElementById(this.btnOpenModalId);
        btnClick.addEventListener("click", (evt) => {
            this.showModal(evt);
        });

        const btnClose = document.getElementsByClassName("onax-closeBtn")[0];
        btnClose.addEventListener("click", this.closeModal);


    };

    showModal = (evt) => {
        this.popupContainer.style.display = "block";
    };

    closeModal = () => {
        this.popupContainer.style.display = "none";
    };

    createStyles = () => {
        const styleTag = document.createElement("style");
        document.head.appendChild(styleTag);

        styleTag.innerHTML = `
        .onax-modal{
            // display: none;
            position: fixed;
            z-index: 1;
            left: 0;
            top:0;
            height: 100%;
            width: 100%; overflow: auto;
            background-color: rgba(0,0,0,0.3);
            animation: on-onax-ModalOpen;
            animation-duration: 1.0s;
        }

        .onax-modal-Content{
            background-color: #f4f4f4;
            margin:10% auto;
            margin-top: ${this.modalStylesOverride.modalTop};
            width: ${this.modalStylesOverride.modalWidth};
            box-shadow: 0 5px 8px 0 rgba(0,0,0,0.3),0 7px 20px 0 rgba(0,0,0,0.5);
        }
        .onax-modal-header h3, .onax-modal-footer h4{
            margin: 0px;
        }
        .onax-modal-header{
            background: ${this.modalStylesOverride.modalHeaderBgColor &&
                this.modalStylesOverride.modalHeaderBgColor.length > 2
                ? this.modalStylesOverride.modalHeaderBgColor
                : "coral"
            };
            padding:15px;
            color:#fff;
        }
        .onax-modal-body{
            padding:15px;
            background: ${this.modalStylesOverride.modalBodyBgColor &&
                this.modalStylesOverride.modalBodyBgColor.length > 2
                ? this.modalStylesOverride.modalBodyBgColor
                : "#fff"
            };
            color:#000;
        }
        .onax-modal-footer{
            background: ${this.modalStylesOverride.modalFooterBgColor &&
                this.modalStylesOverride.modalFooterBgColor.length > 2
                ? this.modalStylesOverride.modalFooterBgColor
                : "coral"
            };
            padding:15px !important;
            color:#fff;
        }

        .onax-closeBtn{
            float:right;
            color:#fff;
            size: 50px;
        }

        .onax-closeBtn:hover, .onax-closeBtn:focus{
            color:#000;
            cursor: pointer;
        }
        @keyframes on-onax-ModalOpen{
            from {opacity: 0;} to {opacity: 1;}
        }
    `;
    };
}

export default ModalPopup;
