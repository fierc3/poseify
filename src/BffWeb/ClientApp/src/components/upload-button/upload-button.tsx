import { BaseSyntheticEvent, FC, useState } from "react";
import { Alert, Backdrop, Button, Checkbox, CircularProgress, Dialog, DialogActions, DialogContent, DialogContentText, DialogTitle, Fab, ListItemText, MenuItem, Select, SelectChangeEvent, Snackbar, Stack, TextField, Typography } from "@mui/material";
import axios from "axios";
import NavigationIcon from '@mui/icons-material/Navigation';

export const UploadButton: FC = () => {

    const [openUpoadDialog, setOpenUploadDialog] = useState(false);

    const handleClickOpen = () => {
        setOpenUploadDialog(true);
    };

    const reset = () =>{
        setEstimationName("");
        setTags([]);
        setSelectedFilePath("");
    }

    const handleClose = () => {
        setOpenUploadDialog(false);
        reset();
    };

    const [tags, setTags] = useState<string[]>([]);
    const [estimationName, setEstimationName] = useState<string>("");
    const [selectedFilePath, setSelectedFilePath] = useState<string>("");
    const [isUploading, setIsUploading] = useState<boolean>(false);
    const [errorSnack, setErrorSnack] = useState<boolean>(false);
    const [successSnack, setSuccessSnack] = useState<boolean>(false);

    const handleChange = (event: SelectChangeEvent<typeof tags>) => {
        const {
            target: { value },
        } = event;
        setTags(
            // On autofill we get a stringified value.
            typeof value === 'string' ? value.split(',') : value,
        );
    };

    const tagOptions = ['Research', 'Sports', 'Dance', 'Animation']

    const onSubmitFile = async () => {
        handleClose();
        setIsUploading(true);
        const inputFile = document.getElementById(
            "fileInput",
        ) as HTMLInputElement;

        const formData = new FormData();
        console.log(inputFile)
        formData.append("FormFile", inputFile?.files?.item(0) as File);
        formData.append("EstimationName", estimationName);
        formData.append("Tags", tags.join(","));


        const res = axios.post<{ url: string }>(
            `/api/PostUpload`,
            formData,
            {
                headers: {
                    "Access-Control-Allow-Origin": "*",
                    'X-CSRF': '1'
                },
            },
        )

        res.catch(x => {
            setTimeout(() => setIsUploading(false), 200);
            setErrorSnack(true);
        });

        const result = await res

        if (result.status === 201) {
            setErrorSnack(true);
        } else {
            setSuccessSnack(true);
        }

        //setValue("thumbnail", res.data.url);
        setTimeout(() => setIsUploading(false), 200);
    };

    return (
        <>
            <Fab onClick={handleClickOpen} variant="extended" size="small" color="primary" aria-label="add">
                <NavigationIcon sx={{ mr: 1 }} />
                Upload Estimation
            </Fab>
            <Backdrop
                sx={{ color: '#fff', zIndex: (theme) => theme.zIndex.drawer + 1 }}
                open={isUploading}
            >
                <CircularProgress color="inherit" />
                <Typography variant="subtitle1" paddingLeft={3}>
                    File is uploading
                </Typography>
            </Backdrop>
            <Snackbar
                anchorOrigin={{
                    vertical: 'top',
                    horizontal: 'right'
                }}
                open={errorSnack}
                onClose={() => setErrorSnack(false)}
                autoHideDuration={6000}
            >
                <Alert severity="error" sx={{ width: '100%' }}>
                    Estimation couldn't be created!
                </Alert>
            </Snackbar>
            <Snackbar
                anchorOrigin={{
                    vertical: 'top',
                    horizontal: 'right'
                }}
                open={successSnack}
                onClose={() => setSuccessSnack(false)}
                autoHideDuration={6000}
            >
                <Alert severity="success" sx={{ width: '100%' }}>
                    Estimation uploaded, now processing!
                </Alert>
            </Snackbar>
            {openUpoadDialog && (<Dialog fullWidth open={openUpoadDialog} onClose={handleClose}>
                <DialogTitle>Upload</DialogTitle>
                <DialogContent>
                    <Typography variant="h5">1. Information</Typography>
                    <DialogContentText>
                        To create a new estimation we need some information.
                    </DialogContentText>
                    <Stack
                        component="form"
                        spacing={2}
                        noValidate
                        autoComplete="off"
                    >
                        <TextField
                            autoFocus
                            margin="dense"
                            id="name"
                            label="Estimation Name"
                            type="text"
                            fullWidth
                            variant="standard"
                            required
                            value={estimationName}
                            onChange={(e) => setEstimationName(e.target.value)}
                        />
                        <Select
                            labelId="demo-multiple-checkbox-label"
                            id="demo-multiple-checkbox"
                            multiple
                            value={tags}
                            onChange={handleChange}
                            label="Tags"
                            renderValue={(selected) => selected.join(', ')}
                            MenuProps={{
                                PaperProps: {
                                    style: {
                                        maxHeight: 48 * 4.5 + 8,
                                        width: 250,
                                    },
                                },
                            }}
                        >
                            {tagOptions.map((tag) => (
                                <MenuItem key={tag} value={tag}>
                                    <Checkbox checked={tags.indexOf(tag) > -1} />
                                    <ListItemText primary={tag} />
                                </MenuItem>
                            ))}
                        </Select>
                        <Typography variant="h5">2. Video</Typography>
                        <DialogContentText>
                            Select the file that should be processed for the estimation
                        </DialogContentText>
                        <Alert variant="filled" severity="success">- USE STATIC VIDEOS WITH SINGLE PERSON IN FRAME</Alert>
                        <Alert variant="filled" severity="success">- USE WELL LIT VIDEOS</Alert>
                        <Alert variant="filled" severity="warning">- DO NOT USE VIDEOS WITH MULTIPLE PEOPLE IN FRAME</Alert>
                        <Button variant="contained" component="label">
                            Upload
                            <input onInput={(x: BaseSyntheticEvent) => setSelectedFilePath(x.target.value.replace(/^.*[\\\/]/, ''))} id="fileInput" hidden accept="video/*" multiple type="file" />
                        </Button>
                        <DialogContentText>
                            {selectedFilePath}
                        </DialogContentText>
                    </Stack>
                </DialogContent>
                <DialogActions>
                    <Button onClick={handleClose}>Cancel</Button>
                    <Button disabled={selectedFilePath.length < 1 || estimationName.length < 1 || tags.length < 1} onClick={onSubmitFile}>Start Upload</Button>
                </DialogActions>
            </Dialog>)}
        </>
    );
};
