import { FC, useEffect, useState } from "react";
import { IconButton, CircularProgress, Dialog, DialogActions, DialogContent, DialogContentText, DialogTitle } from "@mui/material";
import { AttachmentType, IEstimation } from "../../helpers/api.types";
import { Download } from '@mui/icons-material';
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

    const download = (estimation: IEstimation | null, type: AttachmentType) => {
        if (estimation == null) return;

        // create download for specific attachment type
        axios.get(`/api/GetAttachment?estimationId=${estimation.internalGuid}&attachmentType=${type}`, { responseType: 'arraybuffer', headers: { 'X-CSRF': '1' } })
            .then((res) => {
                const url = window.URL.createObjectURL(new Blob([res.data]));
                const link = document.createElement('a');
                link.href = url;
                var fileName = 
                    type === AttachmentType.Joints ? estimation.displayName + '.json' : 
                    type === AttachmentType.Preview ? estimation.displayName + '.mp4' : 
                    type === AttachmentType.Bvh ? estimation.displayName + '.bvh' : 
                    type === AttachmentType.Fbx ? estimation.displayName + '.fbx' : 
                    estimation.displayName + '.npz'
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
                    <DialogActions style={{ justifyContent: "space-between" }}>
                        <IconButton size="small" onClick={() => download(props.estimation, AttachmentType.Joints)} autoFocus>
                            <Download fontSize="small"/>
                            Joints
                        </IconButton>
                        <IconButton size="small" onClick={() => download(props.estimation, AttachmentType.Npz)} autoFocus>
                        <Download fontSize="small"/>
                            NPZ
                        </IconButton>
                        <IconButton size="small" onClick={() => download(props.estimation, AttachmentType.Preview)} autoFocus>
                        <Download fontSize="small"/>
                            Preview
                        </IconButton>
                        <IconButton size="small" onClick={() => download(props.estimation, AttachmentType.Bvh)} autoFocus>
                        <Download fontSize="small"/>
                            BVH
                        </IconButton>
                        <IconButton size="small" onClick={() => download(props.estimation, AttachmentType.Fbx)} autoFocus>
                        <Download fontSize="small"/>
                            FBX
                        </IconButton>
                        <IconButton size="small" onClick={props.handleClose} autoFocus>
                            close
                        </IconButton>
                    </DialogActions>
                </Dialog>
            )}
        </>
    );
};
