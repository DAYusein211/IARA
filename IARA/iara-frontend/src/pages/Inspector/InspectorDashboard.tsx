import { useState, useEffect } from 'react';
import { Navbar } from '../../components/Navbar';
import { useAuth } from '../../context/AuthContext';
import { ClipboardCheck, AlertCircle, DollarSign, TrendingUp } from 'lucide-react';
import { inspectionsAPI } from '../../api/services';
import { Inspection } from '../../types';
import { InspectionForm } from './components/InspectionForm';

export const InspectorDashboard = () => {
  const { user } = useAuth();
  const [myInspections, setMyInspections] = useState<Inspection[]>([]);
  const [allInspections, setAllInspections] = useState<Inspection[]>([]);
  const [unpaidFines, setUnpaidFines] = useState<Inspection[]>([]);
  const [loading, setLoading] = useState(true);
  const [showInspectionForm, setShowInspectionForm] = useState(false);

  useEffect(() => {
    loadDashboardData();
  }, [user]);

  const loadDashboardData = async () => {
    if (!user) return;

    try {
      const [myInspRes, allInspRes, finesRes] = await Promise.all([
        inspectionsAPI.getByInspector(user.id),
        inspectionsAPI.getAll(),
        inspectionsAPI.getUnpaidFines(),
      ]);

      setMyInspections(myInspRes.data);
      setAllInspections(allInspRes.data);
      setUnpaidFines(finesRes.data);
    } catch (error) {
      console.error('Error loading dashboard data:', error);
    } finally {
      setLoading(false);
    }
  };

  const stats = {
    myInspections: myInspections.length,
    totalInspections: allInspections.length,
    failedInspections: allInspections.filter(i => i.result === 'FAILED').length,
    unpaidFines: unpaidFines.length,
    totalFineAmount: unpaidFines.reduce((sum, i) => sum + (i.fine?.amount || 0), 0),
  };

  return (
    <div className="min-h-screen bg-gray-100">
      <Navbar />
      
      <div className="max-w-7xl mx-auto py-6 px-4 sm:px-6 lg:px-8">
        {/* Header */}
        <div className="mb-8">
          <h1 className="text-3xl font-bold text-gray-900">Inspector Dashboard</h1>
          <p className="text-gray-600 mt-2">Manage inspections and enforcement</p>
        </div>

        {loading ? (
          <div className="text-center py-12">
            <div className="inline-block animate-spin rounded-full h-8 w-8 border-4 border-blue-500 border-t-transparent"></div>
            <p className="text-gray-600 mt-4">Loading dashboard...</p>
          </div>
        ) : (
          <>
            {/* Stats Grid */}
            <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-4 gap-6 mb-8">
              <div className="bg-white rounded-lg shadow p-6">
                <div className="flex items-center justify-between">
                  <div>
                    <p className="text-sm font-medium text-gray-600">My Inspections</p>
                    <p className="text-3xl font-bold text-gray-900 mt-2">{stats.myInspections}</p>
                  </div>
                  <ClipboardCheck className="h-8 w-8 text-blue-600" />
                </div>
              </div>

              <div className="bg-white rounded-lg shadow p-6">
                <div className="flex items-center justify-between">
                  <div>
                    <p className="text-sm font-medium text-gray-600">Total Inspections</p>
                    <p className="text-3xl font-bold text-gray-900 mt-2">{stats.totalInspections}</p>
                  </div>
                  <TrendingUp className="h-8 w-8 text-green-600" />
                </div>
              </div>

              <div className="bg-white rounded-lg shadow p-6">
                <div className="flex items-center justify-between">
                  <div>
                    <p className="text-sm font-medium text-gray-600">Failed Inspections</p>
                    <p className="text-3xl font-bold text-gray-900 mt-2">{stats.failedInspections}</p>
                  </div>
                  <AlertCircle className="h-8 w-8 text-red-600" />
                </div>
              </div>

              <div className="bg-white rounded-lg shadow p-6">
                <div className="flex items-center justify-between">
                  <div>
                    <p className="text-sm font-medium text-gray-600">Unpaid Fines</p>
                    <p className="text-3xl font-bold text-gray-900 mt-2">{stats.unpaidFines}</p>
                    <p className="text-xs text-gray-500 mt-1">${stats.totalFineAmount.toFixed(2)}</p>
                  </div>
                  <DollarSign className="h-8 w-8 text-yellow-600" />
                </div>
              </div>
            </div>

            {/* Recent Inspections + Create Inspection */}
            <div className="bg-white rounded-lg shadow">
              <div className="p-6 border-b border-gray-200 flex items-center justify-between">
                <h2 className="text-xl font-semibold text-gray-900">Recent Inspections</h2>
                <button
                  className="bg-blue-600 text-white px-4 py-2 rounded-lg font-semibold hover:bg-blue-700 transition"
                  onClick={() => setShowInspectionForm(true)}
                >
                  + New Inspection
                </button>
              </div>
              <div className="p-6">
                {myInspections.length === 0 ? (
                  <p className="text-gray-600 text-center py-8">No inspections yet.</p>
                ) : (
                  <div className="space-y-4">
                    {myInspections.slice(0, 5).map(inspection => (
                      <div key={inspection.id} className="border border-gray-200 rounded-lg p-4">
                        <div className="flex items-center justify-between">
                          <div className="flex-1">
                            <div className="flex items-center space-x-3">
                              <h3 className="font-semibold text-gray-900">{inspection.targetDescription}</h3>
                              <span className={`inline-flex items-center px-2.5 py-0.5 rounded-full text-xs font-medium ${
                                inspection.result === 'PASSED' ? 'bg-green-100 text-green-800' :
                                inspection.result === 'FAILED' ? 'bg-red-100 text-red-800' :
                                'bg-yellow-100 text-yellow-800'
                              }`}>
                                {inspection.result}
                              </span>
                            </div>
                            <p className="text-sm text-gray-600 mt-1">
                              Date: {new Date(inspection.inspectionDate).toLocaleDateString()}
                            </p>
                            {inspection.fine && (
                              <p className="text-sm text-red-600 mt-1">
                                Fine: ${inspection.fine.amount} - {inspection.fine.isPaid ? 'Paid' : 'Unpaid'}
                              </p>
                            )}
                          </div>
                        </div>
                      </div>
                    ))}
                  </div>
                )}
              </div>
            </div>
            <InspectionForm
              isOpen={showInspectionForm}
              onClose={() => setShowInspectionForm(false)}
              onSuccess={loadDashboardData}
            />

            {/* Unpaid Fines Section */}
            {unpaidFines.length > 0 && (
              <div className="bg-white rounded-lg shadow mt-6">
                <div className="p-6 border-b border-gray-200">
                  <h2 className="text-xl font-semibold text-gray-900">Unpaid Fines</h2>
                </div>
                <div className="p-6">
                  <div className="space-y-4">
                    {unpaidFines.slice(0, 5).map(inspection => (
                      <div key={inspection.id} className="border border-red-200 bg-red-50 rounded-lg p-4">
                        <div className="flex items-center justify-between">
                          <div>
                            <h3 className="font-semibold text-gray-900">{inspection.targetDescription}</h3>
                            <p className="text-sm text-gray-600">Inspector: {inspection.inspectorName}</p>
                            <p className="text-sm text-red-600 font-medium mt-1">
                              Amount: ${inspection.fine?.amount} - {inspection.fine?.reason}
                            </p>
                          </div>
                        </div>
                      </div>
                    ))}
                  </div>
                </div>
              </div>
            )}
          </>
        )}
      </div>
    </div>
  );
};