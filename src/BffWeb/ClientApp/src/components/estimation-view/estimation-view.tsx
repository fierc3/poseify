import { FC, useEffect, useState } from "react";
import { Button, CircularProgress, Dialog, DialogActions, DialogContent, DialogContentText, DialogTitle } from "@mui/material";
import { AttachmentType, IEstimation } from "../../helpers/api.types";
import axios from "axios";

export const EstimationView: FC<{ estimation: IEstimation | null, open: boolean, handleClose: () => void }> = (props) => {

    const [previewUrl, setPreviewUrl] = useState<string | undefined>(undefined);


    useEffect(() => {
        // load preview when possible
        setPreviewUrl(undefined);
        if (props.estimation == null) return;
        axios.get(`/api/GetAttachment?estimationId=${props.estimation.internalGuid}&attachmentType=${AttachmentType.Preview}`, { responseType: 'arraybuffer', headers: { 'X-CSRF': '1' } }).then(res => {
            const url = window.URL.createObjectURL(new Blob([res.data]));
            setPreviewUrl(url);
        })
    }, [props.estimation])

    const download = (estimation: IEstimation | null, type: string) => {
        if (estimation == null) return;

        // create download for specific attachment type
        axios.get(`/api/GetAttachment?estimationId=${estimation.internalGuid}&attachmentType=${type}`, { responseType: 'arraybuffer', headers: { 'X-CSRF': '1' } })
            .then((res) => {
                const url = window.URL.createObjectURL(new Blob([res.data]));
                const link = document.createElement('a');
                link.href = url;
                var fileName = type === AttachmentType.Joints ? estimation.displayName + '.npz' : estimation.displayName + '.mp4'
                link.setAttribute('download', fileName); //or any other extension
                document.body.appendChild(link);
                link.click();
            });
    }


    return (
        <>
            {props.estimation != null && (
                <Dialog
                    open={props.open}
                    onClose={props.handleClose}
                    aria-labelledby="alert-dialog-title"
                    aria-describedby="alert-dialog-description"
                >
                    <DialogTitle id="alert-dialog-title">
                        Estimation: {props.estimation.displayName}
                    </DialogTitle>
                    <DialogContent>
                        <DialogContentText id="alert-dialog-description" textAlign={"center"}>
                            {previewUrl ?(<video autoPlay width={500} loop>
                                <source src={previewUrl} type="video/mp4" />
                            </video>) : <CircularProgress/>}
                        </DialogContentText>
                    </DialogContent>
                    <DialogActions>
                        <Button onClick={() => download(props.estimation, AttachmentType.Joints)} autoFocus>
                            Download Joint Positions
                        </Button>
                        <Button onClick={() => download(props.estimation, AttachmentType.Preview)} autoFocus>
                            Download Preview
                        </Button>
                        <Button onClick={props.handleClose} autoFocus>
                            close
                        </Button>
                    </DialogActions>
                </Dialog>
            )}
        </>
    );
};
