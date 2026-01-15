import { Link, useNavigate } from 'react-router-dom';
import { useAuth } from '../context/AuthContext';
import { Ship, LogOut, BarChart3, User } from 'lucide-react';

export const Navbar = () => {
  const { user, logout, isAuthenticated } = useAuth();
  const navigate = useNavigate();

  const handleLogout = () => {
    logout();
    navigate('/login');
  };

  if (!isAuthenticated) {
    return null;
  }

  return (
    <nav className="bg-blue-600 text-white shadow-lg">
      <div className="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8">
        <div className="flex justify-between h-16">
          <div className="flex items-center space-x-8">
            <div className="flex items-center space-x-2 select-none">
              <Ship className="h-8 w-8" />
              <span className="font-bold text-xl">IARA</span>
            </div>

            <div className="hidden md:flex space-x-4">
              <Link
                to="/dashboard"
                className="flex items-center space-x-1 px-3 py-2 rounded-md hover:bg-blue-700 transition text-white hover:text-white"
              >
                <BarChart3 className="h-4 w-4" />
                <span>Dashboard</span>
              </Link>

              {(user?.role === 'ADMIN' || user?.role === 'INSPECTOR') && (
                <Link
                  to="/reports"
                  className="flex items-center space-x-1 px-3 py-2 rounded-md hover:bg-blue-700 transition text-white hover:text-white"
                >
                  <BarChart3 className="h-4 w-4" />
                  <span>Reports</span>
                </Link>
              )}
            </div>
          </div>

          <div className="flex items-center space-x-4">
            <div className="flex items-center space-x-2">
              <User className="h-5 w-5" />
              <div className="hidden md:block">
                <p className="text-sm font-medium">{user?.fullName}</p>
                <p className="text-xs text-blue-200">{user?.role.replace('_', ' ')}</p>
              </div>
            </div>

            <button
              onClick={handleLogout}
              className="flex items-center space-x-1 px-4 py-2 bg-red-600 rounded-md hover:bg-red-700 transition text-white"
            >
              <LogOut className="h-4 w-4" />
              <span>Logout</span>
            </button>
          </div>
        </div>
      </div>
    </nav>
  );
};