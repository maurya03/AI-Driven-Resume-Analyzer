import React, {useState} from 'react';
import api from '../api';
import { TextField, Button } from '@mui/material';
export default function Upload(){
  const [name,setName]=useState(''); const [email,setEmail]=useState(''); const [exp,setExp]=useState(0); const [file,setFile]=useState(null);
  const handle=async(e)=>{ e.preventDefault(); const fd=new FormData(); fd.append('CandidateName',name); fd.append('Email',email); fd.append('ExperienceYears',exp); fd.append('ResumeFile',file); try{ await api.post('/resumes/upload',fd); alert('Uploaded'); }catch(e){console.error(e); alert('Upload failed'); } };
  return (<form onSubmit={handle} style={{maxWidth:600}}>
    <h2>Upload Resume</h2>
    <TextField label="Name" value={name} onChange={e=>setName(e.target.value)} fullWidth sx={{mb:1}} />
    <TextField label="Email" value={email} onChange={e=>setEmail(e.target.value)} fullWidth sx={{mb:1}} />
    <TextField label="ExperienceYears" type="number" value={exp} onChange={e=>setExp(e.target.value)} fullWidth sx={{mb:1}} />
    <input type="file" onChange={e=>setFile(e.target.files[0])} style={{marginBottom:12}} />
    <Button type="submit" variant="contained">Upload</Button>
  </form>);
}
