import { BrowserRouter, Routes, Route, Navigate } from 'react-router-dom';
import { AuthProvider, useAuth } from './context/AuthContext';
import { ProtectedRoute } from './components/ProtectedRoute';
import { Login } from './pages/Login';
import { Register } from './pages/Register';
import { AdminDashboard } from './pages/Admin/AdminDashboard';
import { FisherDashboard } from './pages/Fisher/FisherDashboard';
import { InspectorDashboard } from './pages/Inspector/InspectorDashboard';
import { RecreationalDashboard } from './pages/Recreational/RecreationalDashboard';
import { ReportsPage } from './pages/Reports/ReportsPage';
import { UserRole } from './types';

// Dashboard Router component that redirects based on role
const DashboardRouter = () => {
  const { user } = useAuth();

  if (!user) return <Navigate to="/login" replace />;

  switch (user.role) {
    case UserRole.ADMIN:
      return <AdminDashboard />;
    case UserRole.INSPECTOR:
      return <InspectorDashboard />;
    case UserRole.PROFESSIONAL_FISHER:
      return <FisherDashboard />;
    case UserRole.RECREATIONAL_FISHER:
      return <RecreationalDashboard />;
    default:
      return <Navigate to="/login" replace />;
  }
};

function App() {
  return (
    <AuthProvider>
      <BrowserRouter>
        <Routes>
          <Route path="/login" element={<Login />} />
          <Route path="/register" element={<Register />} />
          
          <Route
            path="/dashboard"
            element={
              <ProtectedRoute>
                <DashboardRouter />
              </ProtectedRoute>
            }
          />

          <Route
            path="/reports"
            element={
              <ProtectedRoute allowedRoles={[UserRole.ADMIN, UserRole.INSPECTOR]}>
                <ReportsPage />
              </ProtectedRoute>
            }
          />
          
          <Route path="/" element={<Navigate to="/dashboard" replace />} />
          
          <Route
            path="*"
            element={
              <div className="min-h-screen bg-gray-100 flex items-center justify-center">
                <div className="text-center">
                  <h1 className="text-6xl font-bold text-gray-900">404</h1>
                  <p className="text-xl text-gray-600 mt-4">Page not found</p>
                </div>
              </div>
            }
          />
        </Routes>
      </BrowserRouter>
    </AuthProvider>
  );
}

export default App;