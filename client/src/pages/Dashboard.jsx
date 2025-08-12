import React, {useEffect, useState} from 'react';
import api from '../api';
import ResumeTable from '../components/ResumeTable';
import { TextField, Button } from '@mui/material';
export default function Dashboard(){
  const [resumes,setResumes]=useState([]);
  const [query,setQuery]=useState('');
  const fetchAll=async()=>{ try{ const r=await api.get('/resumes'); setResumes(r.data); }catch(e){console.error(e);} };
  useEffect(()=>{ fetchAll(); },[]);
  const doSearch=async()=>{ try{ const r=await api.post('/resumes/search',{query,topK:10}); setResumes(r.data);}catch(e){console.error(e);} };
  return (<div>
    <h2>Resume AI Dashboard</h2>
    <div style={{display:'flex',gap:8,marginBottom:12}}>
      <TextField label="Semantic search" value={query} onChange={e=>setQuery(e.target.value)} fullWidth />
      <Button variant="contained" onClick={doSearch}>Search</Button>
      <Button variant="outlined" onClick={fetchAll}>Refresh</Button>
    </div>
    <ResumeTable resumes={resumes} />
  </div>);
}
