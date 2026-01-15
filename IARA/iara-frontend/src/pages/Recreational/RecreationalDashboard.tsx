import { useState, useEffect } from 'react';
import { Navbar } from '../../components/Navbar';
import { useAuth } from '../../context/AuthContext';
import { Fish, Calendar, CreditCard, CheckCircle } from 'lucide-react';
import { ticketsAPI } from '../../api/services';
import { Ticket, TicketType } from '../../types';

export const RecreationalDashboard = () => {
  const { user } = useAuth();
  const [tickets, setTickets] = useState<Ticket[]>([]);
  const [activeTicket, setActiveTicket] = useState<Ticket | null>(null);
  const [loading, setLoading] = useState(true);
  const [buying, setBuying] = useState(false);
  const [selectedType, setSelectedType] = useState<TicketType>(TicketType.DAILY);

  useEffect(() => {
    loadDashboardData();
  }, [user]);

  const loadDashboardData = async () => {
    if (!user) return;

    try {
      const [ticketsRes, activeRes] = await Promise.all([
        ticketsAPI.getByUser(user.id),
        ticketsAPI.getActiveForUser(user.id).catch(() => ({ data: null })),
      ]);

      setTickets(ticketsRes.data);
      setActiveTicket(activeRes.data);
    } catch (error) {
      console.error('Error loading dashboard data:', error);
    } finally {
      setLoading(false);
    }
  };

  const handleBuyTicket = async () => {
    if (!user) return;

    setBuying(true);
    try {
      await ticketsAPI.buy({ userId: user.id, ticketType: selectedType });
      await loadDashboardData();
      alert('Ticket purchased successfully!');
    } catch (error: any) {
      alert(error.response?.data?.message || 'Failed to purchase ticket');
    } finally {
      setBuying(false);
    }
  };

  const ticketPrices = {
    [TicketType.DAILY]: 10,
    [TicketType.WEEKLY]: 50,
    [TicketType.MONTHLY]: 150,
    [TicketType.YEARLY]: 1200,
  };

  return (
    <div className="min-h-screen bg-gray-100">
      <Navbar />
      
      <div className="max-w-7xl mx-auto py-6 px-4 sm:px-6 lg:px-8">
        {/* Header */}
        <div className="mb-8">
          <h1 className="text-3xl font-bold text-gray-900">Recreational Fishing</h1>
          <p className="text-gray-600 mt-2">Manage your fishing tickets</p>
        </div>

        {loading ? (
          <div className="text-center py-12">
            <div className="inline-block animate-spin rounded-full h-8 w-8 border-4 border-blue-500 border-t-transparent"></div>
            <p className="text-gray-600 mt-4">Loading dashboard...</p>
          </div>
        ) : (
          <>
            {/* Active Ticket Card */}
            {activeTicket ? (
              <div className="bg-gradient-to-r from-green-500 to-green-600 rounded-lg shadow-lg p-6 mb-8 text-white">
                <div className="flex items-center justify-between">
                  <div>
                    <div className="flex items-center space-x-2 mb-2">
                      <CheckCircle className="h-6 w-6" />
                      <h2 className="text-2xl font-bold">Active Fishing Ticket</h2>
                    </div>
                    <p className="text-green-100 mb-4">Type: {activeTicket.ticketType}</p>
                    <div className="space-y-1">
                      <p className="text-sm">Valid from: {new Date(activeTicket.validFrom).toLocaleDateString()}</p>
                      <p className="text-sm">Valid until: {new Date(activeTicket.validTo).toLocaleDateString()}</p>
                      <p className="text-sm font-semibold">{activeTicket.daysRemaining} days remaining</p>
                    </div>
                  </div>
                  <Fish className="h-24 w-24 opacity-20" />
                </div>
              </div>
            ) : (
              <div className="bg-yellow-50 border border-yellow-200 rounded-lg p-6 mb-8">
                <div className="flex items-center space-x-3">
                  <Calendar className="h-6 w-6 text-yellow-600" />
                  <div>
                    <h3 className="font-semibold text-gray-900">No Active Ticket</h3>
                    <p className="text-sm text-gray-600">You need an active fishing ticket to fish</p>
                  </div>
                </div>
              </div>
            )}

            {/* Buy Ticket Section */}
            <div className="bg-white rounded-lg shadow p-6 mb-8">
              <h2 className="text-xl font-semibold text-gray-900 mb-4">Purchase Fishing Ticket</h2>
              
              <div className="grid grid-cols-1 md:grid-cols-4 gap-4 mb-6">
                {Object.values(TicketType).map(type => (
                  <button
                    key={type}
                    onClick={() => setSelectedType(type)}
                    className={`p-4 border-2 rounded-lg transition ${
                      selectedType === type
                        ? 'border-blue-500 bg-blue-50'
                        : 'border-gray-200 hover:border-blue-300'
                    }`}
                  >
                    <p className="font-semibold text-gray-900">{type}</p>
                    <p className="text-2xl font-bold text-blue-600 mt-2">${ticketPrices[type]}</p>
                  </button>
                ))}
              </div>

              <button
                onClick={handleBuyTicket}
                disabled={buying}
                className="w-full md:w-auto px-6 py-3 bg-blue-600 text-white font-semibold rounded-lg hover:bg-blue-700 transition disabled:bg-blue-300 disabled:cursor-not-allowed flex items-center justify-center space-x-2"
              >
                <CreditCard className="h-5 w-5" />
                <span>{buying ? 'Processing...' : `Buy ${selectedType} Ticket`}</span>
              </button>
            </div>

            {/* Ticket History */}
            <div className="bg-white rounded-lg shadow">
              <div className="p-6 border-b border-gray-200">
                <h2 className="text-xl font-semibold text-gray-900">Ticket History</h2>
              </div>
              <div className="p-6">
                {tickets.length === 0 ? (
                  <p className="text-gray-600 text-center py-8">No tickets purchased yet.</p>
                ) : (
                  <div className="space-y-4">
                    {tickets.map(ticket => (
                      <div key={ticket.id} className="border border-gray-200 rounded-lg p-4">
                        <div className="flex items-center justify-between">
                          <div>
                            <div className="flex items-center space-x-3">
                              <h3 className="font-semibold text-gray-900">{ticket.ticketType}</h3>
                              <span className={`inline-flex items-center px-2.5 py-0.5 rounded-full text-xs font-medium ${
                                ticket.isActive ? 'bg-green-100 text-green-800' : 'bg-gray-100 text-gray-800'
                              }`}>
                                {ticket.isActive ? 'Active' : 'Expired'}
                              </span>
                            </div>
                            <p className="text-sm text-gray-600 mt-1">
                              Valid: {new Date(ticket.validFrom).toLocaleDateString()} - {new Date(ticket.validTo).toLocaleDateString()}
                            </p>
                            <p className="text-sm text-gray-600">Price: ${ticket.price}</p>
                          </div>
                        </div>
                      </div>
                    ))}
                  </div>
                )}
              </div>
            </div>
          </>
        )}
      </div>
    </div>
  );
};