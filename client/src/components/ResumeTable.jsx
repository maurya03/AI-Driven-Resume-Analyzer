import React, {useState} from 'react';
import { Table, TableHead, TableRow, TableCell, TableBody, Paper, Chip, Dialog, DialogTitle, DialogContent, DialogActions, Button, Typography, Box } from '@mui/material';
export default function ResumeTable({resumes}){
  const [sel,setSel]=useState(null); const [open,setOpen]=useState(false);
  const openRow=(r)=>{ setSel(r); setOpen(true); }; const close=()=>setOpen(false);
  return (<>
    <Paper><Table>
      <TableHead><TableRow><TableCell>Name</TableCell><TableCell>Email</TableCell><TableCell>Exp</TableCell><TableCell>Score</TableCell><TableCell>Skills</TableCell></TableRow></TableHead>
      <TableBody>{(resumes||[]).map(r=>(<TableRow key={r.id} hover onClick={()=>openRow(r)} sx={{cursor:'pointer'}}><TableCell>{r.candidateName}</TableCell><TableCell>{r.email}</TableCell><TableCell>{r.experienceYears}</TableCell><TableCell><Chip label={r.ai_Score} color={r.ai_Score>80?'success':r.ai_Score>50?'warning':'error'} /></TableCell><TableCell>{(r.ai_Skills||'').split(',').map((s,i)=>(<Chip key={i} label={s.trim()} sx={{m:0.3}}/>))}</TableCell></TableRow>))}</TableBody>
    </Table></Paper>
    <Dialog open={open} onClose={close} fullWidth maxWidth="md">
      <DialogTitle>{sel?.candidateName}</DialogTitle>
      <DialogContent dividers>
        <Typography variant="subtitle2">AI Summary</Typography>
        <Typography sx={{whiteSpace:'pre-wrap'}}>{sel?.ai_Summary}</Typography>
        <Box mt={2}><Typography variant="subtitle2">Parsed Text</Typography><Typography sx={{whiteSpace:'pre-wrap'}}>{sel?.rawText}</Typography></Box>
      </DialogContent>
      <DialogActions><Button onClick={close}>Close</Button></DialogActions>
    </Dialog>
  </>);
}
