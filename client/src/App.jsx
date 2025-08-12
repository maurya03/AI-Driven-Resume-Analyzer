import React from 'react';
import { Routes, Route, Link } from 'react-router-dom';
import Dashboard from './pages/Dashboard';
import Upload from './pages/Upload';
export default function App(){
  return (
    <div style={{padding:20}}>
      <nav style={{marginBottom:20}}><Link to="/">Dashboard</Link> | <Link to="/upload">Upload</Link></nav>
      <Routes>
        <Route path="/" element={<Dashboard/>} />
        <Route path="/upload" element={<Upload/>} />
      </Routes>
    </div>
  );
}
