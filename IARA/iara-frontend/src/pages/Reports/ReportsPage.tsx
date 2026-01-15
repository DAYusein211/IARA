import { useState, useEffect } from 'react';
import { Navbar } from '../../components/Navbar';
import { BarChart, Bar, XAxis, YAxis, CartesianGrid, Tooltip, Legend, ResponsiveContainer, PieChart, Pie, Cell } from 'recharts';
import { reportsAPI } from '../../api/services';
import { ExpiringPermitReport, ShipStatistics, CarbonFootprint } from '../../types';
import { AlertTriangle, Ship, TrendingDown, Calendar } from 'lucide-react';

export const ReportsPage = () => {
  const [activeTab, setActiveTab] = useState<'permits' | 'statistics' | 'carbon'>('permits');
  const [expiringPermits, setExpiringPermits] = useState<ExpiringPermitReport[]>([]);
  const [shipStats, setShipStats] = useState<ShipStatistics[]>([]);
  const [carbonData, setCarbonData] = useState<CarbonFootprint[]>([]);
  const [loading, setLoading] = useState(true);
  const currentYear = new Date().getFullYear();

  useEffect(() => {
    loadReports();
  }, []);

  const loadReports = async () => {
    try {
      const [permitsRes, statsRes, carbonRes] = await Promise.all([
        reportsAPI.getExpiringPermits(30),
        reportsAPI.getAllShipsStatistics(currentYear),
        reportsAPI.getCarbonFootprint(),
      ]);

      setExpiringPermits(permitsRes.data);
      setShipStats(statsRes.data);
      setCarbonData(carbonRes.data);
    } catch (error) {
      console.error('Error loading reports:', error);
    } finally {
      setLoading(false);
    }
  };

  const tabs = [
    { id: 'permits' as const, label: 'Expiring Permits', icon: AlertTriangle },
    { id: 'statistics' as const, label: 'Ship Statistics', icon: Ship },
    { id: 'carbon' as const, label: 'Carbon Footprint', icon: TrendingDown },
  ];

  const COLORS = ['#10b981', '#3b82f6', '#f59e0b', '#ef4444', '#8b5cf6'];

  if (loading) {
    return (
      <div className="min-h-screen bg-gray-100">
        <Navbar />
        <div className="max-w-7xl mx-auto py-6 px-4">
          <div className="text-center py-12">
            <div className="inline-block animate-spin rounded-full h-8 w-8 border-4 border-blue-500 border-t-transparent"></div>
            <p className="text-gray-600 mt-4">Loading reports...</p>
          </div>
        </div>
      </div>
    );
  }

  return (
    <div className="min-h-screen bg-gray-100">
      <Navbar />
      
      <div className="max-w-7xl mx-auto py-6 px-4 sm:px-6 lg:px-8">
        <div className="mb-8">
          <h1 className="text-3xl font-bold text-gray-900">Reports & Analytics</h1>
          <p className="text-gray-600 mt-2">System insights and data visualization</p>
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
                    ? ' text-blue-600'
                    : '  text-gray-500 hover:text-gray-700'
                } flex row gap-2 bg-white whitespace-nowrap py-4 px-4 border-b-2 font-medium text-sm`}
              >
                <tab.icon className="h-5 w-5" />
                <span>{tab.label}</span>
              </button>
            ))}
          </nav>
        </div>

        {/* Expiring Permits Tab */}
        {activeTab === 'permits' && (
          <div>
            <div className="bg-white rounded-lg shadow p-6 mb-6">
              <h2 className="text-xl font-semibold text-gray-900 mb-4">
                Permits Expiring in Next 30 Days
              </h2>
              {expiringPermits.length === 0 ? (
                <p className="text-gray-600 text-center py-8">No expiring permits in the next 30 days</p>
              ) : (
                <div className="overflow-x-auto">
                  <table className="min-w-full divide-y divide-gray-200">
                    <thead className="bg-gray-50">
                      <tr>
                        <th className="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase">Ship</th>
                        <th className="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase">Owner</th>
                        <th className="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase">Expires</th>
                        <th className="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase">Days Left</th>
                        <th className="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase">Allowed Gear</th>
                      </tr>
                    </thead>
                    <tbody className="bg-white divide-y divide-gray-200">
                      {expiringPermits.map((permit) => (
                        <tr key={permit.permitId} className="hover:bg-gray-50">
                          <td className="px-6 py-4 whitespace-nowrap">
                            <div className="font-medium text-gray-900">{permit.shipName}</div>
                            <div className="text-sm text-gray-500">{permit.shipRegistrationNumber}</div>
                          </td>
                          <td className="px-6 py-4 whitespace-nowrap">
                            <div className="text-sm text-gray-900">{permit.ownerName}</div>
                            <div className="text-sm text-gray-500">{permit.ownerEmail}</div>
                          </td>
                          <td className="px-6 py-4 whitespace-nowrap text-sm text-gray-600">
                            {new Date(permit.validTo).toLocaleDateString()}
                          </td>
                          <td className="px-6 py-4 whitespace-nowrap">
                            <span className={`inline-flex items-center px-2.5 py-0.5 rounded-full text-xs font-medium ${
                              permit.daysUntilExpiry < 7 ? 'bg-red-100 text-red-800' :
                              permit.daysUntilExpiry < 14 ? 'bg-yellow-100 text-yellow-800' :
                              'bg-green-100 text-green-800'
                            }`}>
                              {permit.daysUntilExpiry} days
                            </span>
                          </td>
                          <td className="px-6 py-4 text-sm text-gray-600">{permit.allowedGear}</td>
                        </tr>
                      ))}
                    </tbody>
                  </table>
                </div>
              )}
            </div>
          </div>
        )}

        {/* Ship Statistics Tab */}
        {activeTab === 'statistics' && (
          <div>
            <div className="bg-white rounded-lg shadow p-6 mb-6">
              <h2 className="text-xl font-semibold text-gray-900 mb-6">
                Ship Performance - {currentYear}
              </h2>
              
              {shipStats.length === 0 ? (
                <p className="text-gray-600 text-center py-8">No ship statistics available</p>
              ) : (
                <>
                  <div className="mb-8">
                    <h3 className="text-lg font-medium text-gray-900 mb-4">Yearly Catch by Ship (kg)</h3>
                    <ResponsiveContainer width="100%" height={300}>
                      <BarChart data={shipStats.slice(0, 10)}>
                        <CartesianGrid strokeDasharray="3 3" />
                        <XAxis dataKey="shipName" angle={-45} textAnchor="end" height={100} />
                        <YAxis />
                        <Tooltip />
                        <Legend />
                        <Bar dataKey="yearlyCatchKg" fill="#3b82f6" name="Catch (kg)" />
                      </BarChart>
                    </ResponsiveContainer>
                  </div>

                  <div className="mb-8">
                    <h3 className="text-lg font-medium text-gray-900 mb-4">Trip Statistics</h3>
                    <ResponsiveContainer width="100%" height={300}>
                      <BarChart data={shipStats.slice(0, 10)}>
                        <CartesianGrid strokeDasharray="3 3" />
                        <XAxis dataKey="shipName" angle={-45} textAnchor="end" height={100} />
                        <YAxis />
                        <Tooltip />
                        <Legend />
                        <Bar dataKey="completedTrips" fill="#10b981" name="Completed Trips" />
                        <Bar dataKey="activeTrips" fill="#f59e0b" name="Active Trips" />
                      </BarChart>
                    </ResponsiveContainer>
                  </div>

                  <div>
                    <h3 className="text-lg font-medium text-gray-900 mb-4">Detailed Statistics</h3>
                    <div className="overflow-x-auto">
                      <table className="min-w-full divide-y divide-gray-200">
                        <thead className="bg-gray-50">
                          <tr>
                            <th className="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase">Ship</th>
                            <th className="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase">Trips</th>
                            <th className="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase">Avg Duration</th>
                            <th className="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase">Total Catch</th>
                            <th className="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase">Fuel Used</th>
                          </tr>
                        </thead>
                        <tbody className="bg-white divide-y divide-gray-200">
                          {shipStats.map((stat) => (
                            <tr key={stat.shipId} className="hover:bg-gray-50">
                              <td className="px-6 py-4 whitespace-nowrap">
                                <div className="font-medium text-gray-900">{stat.shipName}</div>
                                <div className="text-sm text-gray-500">{stat.registrationNumber}</div>
                              </td>
                              <td className="px-6 py-4 whitespace-nowrap text-sm text-gray-600">
                                {stat.completedTrips} / {stat.totalTrips}
                              </td>
                              <td className="px-6 py-4 whitespace-nowrap text-sm text-gray-600">
                                {stat.averageTripDurationHours?.toFixed(1) || 'N/A'} hrs
                              </td>
                              <td className="px-6 py-4 whitespace-nowrap text-sm text-gray-600">
                                {stat.yearlyCatchKg.toFixed(1)} kg
                              </td>
                              <td className="px-6 py-4 whitespace-nowrap text-sm text-gray-600">
                                {stat.totalFuelUsed?.toFixed(1) || 'N/A'} L
                              </td>
                            </tr>
                          ))}
                        </tbody>
                      </table>
                    </div>
                  </div>
                </>
              )}
            </div>
          </div>
        )}

        {/* Carbon Footprint Tab */}
        {activeTab === 'carbon' && (
          <div>
            <div className="bg-white rounded-lg shadow p-6 mb-6">
              <h2 className="text-xl font-semibold text-gray-900 mb-6">
                Carbon Footprint Analysis
              </h2>
              
              {carbonData.length === 0 ? (
                <p className="text-gray-600 text-center py-8">No carbon footprint data available</p>
              ) : (
                <>
                  <div className="mb-8">
                    <h3 className="text-lg font-medium text-gray-900 mb-4">Fuel Efficiency (L/kg)</h3>
                    <ResponsiveContainer width="100%" height={300}>
                      <BarChart data={carbonData}>
                        <CartesianGrid strokeDasharray="3 3" />
                        <XAxis dataKey="shipName" angle={-45} textAnchor="end" height={100} />
                        <YAxis />
                        <Tooltip />
                        <Legend />
                        <Bar dataKey="fuelPerCatchRatio" fill="#ef4444" name="Fuel per Catch (L/kg)" />
                      </BarChart>
                    </ResponsiveContainer>
                    <p className="text-sm text-gray-600 mt-2">Lower is better - less fuel per kilogram of catch</p>
                  </div>

                  <div>
                    <h3 className="text-lg font-medium text-gray-900 mb-4">Efficiency Ratings</h3>
                    <div className="grid grid-cols-1 md:grid-cols-2 gap-4">
                      {carbonData.map((data) => (
                        <div key={data.shipId} className="border border-gray-200 rounded-lg p-4">
                          <div className="flex items-center justify-between mb-2">
                            <h4 className="font-semibold text-gray-900">{data.shipName}</h4>
                            <span className={`px-3 py-1 rounded-full text-sm font-medium ${
                              data.efficiencyRating === 'Excellent' ? 'bg-green-100 text-green-800' :
                              data.efficiencyRating === 'Good' ? 'bg-blue-100 text-blue-800' :
                              data.efficiencyRating === 'Average' ? 'bg-yellow-100 text-yellow-800' :
                              'bg-red-100 text-red-800'
                            }`}>
                              {data.efficiencyRating}
                            </span>
                          </div>
                          <div className="space-y-1 text-sm text-gray-600">
                            <p>Fuel Used: {data.totalFuelUsed.toFixed(1)} L</p>
                            <p>Total Catch: {data.totalCatchKg.toFixed(1)} kg</p>
                            <p>Ratio: {data.fuelPerCatchRatio.toFixed(2)} L/kg</p>
                          </div>
                        </div>
                      ))}
                    </div>
                  </div>
                </>
              )}
            </div>
          </div>
        )}
      </div>
    </div>
  );
};