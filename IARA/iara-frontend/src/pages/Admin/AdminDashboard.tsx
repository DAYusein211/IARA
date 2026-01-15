import { useState, useEffect } from 'react';
import { Navbar } from '../../components/Navbar';
import { Ship, FileCheck, AlertTriangle, Users } from 'lucide-react';
import { shipsAPI, permitsAPI, inspectionsAPI } from '../../api/services';
import { ShipsManager } from './components/ShipsManager';
import { PermitsManager } from './components/PermitsManager';

export const AdminDashboard = () => {
  const [activeTab, setActiveTab] = useState<'overview' | 'ships' | 'permits'>('overview');
  const [stats, setStats] = useState({
    totalShips: 0,
    expiringPermits: 0,
    unpaidFines: 0,
    totalUsers: 0,
  });
  const [loading, setLoading] = useState(true);

  useEffect(() => {
    if (activeTab === 'overview') {
      loadDashboardData();
    }
  }, [activeTab]);

  const loadDashboardData = async () => {
    try {
      const [shipsRes, permitsRes, finesRes] = await Promise.all([
        shipsAPI.getAll(),
        permitsAPI.getExpiring(30),
        inspectionsAPI.getUnpaidFines(),
      ]);

      setStats({
        totalShips: shipsRes.data.length,
        expiringPermits: permitsRes.data.length,
        unpaidFines: finesRes.data.length,
        totalUsers: 0,
      });
    } catch (error) {
      console.error('Error loading dashboard data:', error);
    } finally {
      setLoading(false);
    }
  };

  const tabs = [
    { id: 'overview' as const, label: 'Overview' },
    { id: 'ships' as const, label: 'Manage Ships' },
    { id: 'permits' as const, label: 'Manage Permits' },
  ];

  return (
    <div className="min-h-screen bg-gray-100">
      <Navbar />
      
      <div className="max-w-7xl mx-auto py-6 px-4 sm:px-6 lg:px-8">
        {/* Header */}
        <div className="mb-8">
          <h1 className="text-3xl font-bold text-gray-900">Admin Dashboard</h1>
          <p className="text-gray-600 mt-2">System overview and management</p>
        </div>

        {/* Tabs */}
        <div className="border-b border-gray-200 mb-6">
          <nav className="-mb-px flex space-x-8">
            {tabs.map(tab => (
              <button
                key={tab.id}
                onClick={() => setActiveTab(tab.id)}
                className={`${
                  activeTab === tab.id
                    ? 'text-blue-600'
                    : ' text-gray-500 hover:text-gray-700'
                } bg-white whitespace-nowrap py-4 px-4 border-b-2 font-medium text-sm`}
              >
                {tab.label}
              </button>
            ))}
          </nav>
        </div>

        {/* Tab Content */}
        {activeTab === 'overview' && (
          <>
            {loading ? (
              <div className="text-center py-12">
                <div className="inline-block animate-spin rounded-full h-8 w-8 border-4 border-blue-500 border-t-transparent"></div>
                <p className="text-gray-600 mt-4">Loading dashboard...</p>
              </div>
            ) : (
              <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-4 gap-6">
                <div className="bg-white rounded-lg shadow p-6">
                  <div className="flex items-center justify-between">
                    <div>
                      <p className="text-sm font-medium text-gray-600">Total Ships</p>
                      <p className="text-3xl font-bold text-gray-900 mt-2">{stats.totalShips}</p>
                    </div>
                    <Ship className="h-8 w-8 text-blue-600" />
                  </div>
                </div>

                <div className="bg-white rounded-lg shadow p-6">
                  <div className="flex items-center justify-between">
                    <div>
                      <p className="text-sm font-medium text-gray-600">Expiring Permits</p>
                      <p className="text-3xl font-bold text-gray-900 mt-2">{stats.expiringPermits}</p>
                    </div>
                    <AlertTriangle className="h-8 w-8 text-yellow-600" />
                  </div>
                </div>

                <div className="bg-white rounded-lg shadow p-6">
                  <div className="flex items-center justify-between">
                    <div>
                      <p className="text-sm font-medium text-gray-600">Unpaid Fines</p>
                      <p className="text-3xl font-bold text-gray-900 mt-2">{stats.unpaidFines}</p>
                    </div>
                    <FileCheck className="h-8 w-8 text-red-600" />
                  </div>
                </div>

                <div className="bg-white rounded-lg shadow p-6">
                  <div className="flex items-center justify-between">
                    <div>
                      <p className="text-sm font-medium text-gray-600">Total Users</p>
                      <p className="text-3xl font-bold text-gray-900 mt-2">{stats.totalUsers}</p>
                    </div>
                    <Users className="h-8 w-8 text-green-600" />
                  </div>
                </div>
              </div>
            )}
          </>
        )}

        {activeTab === 'ships' && <ShipsManager />}
        {activeTab === 'permits' && <PermitsManager />}
      </div>
    </div>
  );
};