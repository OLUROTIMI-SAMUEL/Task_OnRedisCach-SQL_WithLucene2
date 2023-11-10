


using Lucene.Net.Analysis.Core;
using Lucene.Net.Analysis.Standard;
using Lucene.Net.Analysis;
using Lucene.Net.Documents;
using Lucene.Net.Index;
using Lucene.Net.Search;
using Lucene.Net.Store;
using Microsoft.EntityFrameworkCore;
using Task_1.Models;
using Lucene.Net.QueryParsers.Classic;
using Task_1.Db_Folder;

public class LuceneService
{
    private const string IndexPath = "LuceneIndex";
    private readonly CustomerInfo_DbContext _dbContext;

    public LuceneService(CustomerInfo_DbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public void IndexCustomers()
    {

        FSDirectory directory = FSDirectory.Open(new DirectoryInfo(IndexPath));
        IndexWriterConfig config = new IndexWriterConfig(Lucene.Net.Util.LuceneVersion.LUCENE_48, new StandardAnalyzer(Lucene.Net.Util.LuceneVersion.LUCENE_48));

        using (IndexWriter writer = new IndexWriter(directory, config))
        {
            using (IndexReader reader = DirectoryReader.Open(writer, true))
            {
                IndexSearcher searcher = new IndexSearcher(reader);

                foreach (var customer in _dbContext.CustomersInfo)
                {
                    // Check if a document with the same ID already exists in the index
                    TermQuery termQuery = new TermQuery(new Term("Id", customer.Id.ToString()));
                    TopDocs existingDocs = searcher.Search(termQuery, 1);

                    if (existingDocs.TotalHits > 0)
                    {
                        // Update the existing document
                        int existingDocId = existingDocs.ScoreDocs[0].Doc;
                        Document existingDoc = searcher.Doc(existingDocId);
                        writer.DeleteDocuments(new Term("Id", customer.Id.ToString())); // Delete the existing document
                        IndexCustomer(writer, customer); // Index the updated document
                    }
                    else
                    {
                        // Add a new document
                        IndexCustomer(writer, customer);
                    }
                }
            }
        }
    }

    public List<Customer_Information> SearchById(int id)
    {
        return Search("Id", id.ToString());
    }

    public List<Customer_Information> SearchByName(string name)
    {
        return Search("Name", name);
    }

    public List<Customer_Information> SearchByAddress(string address)
    {
        return Search("Address", address);
    }

    public List<Customer_Information> SearchByEmail(string email)
    {
        return Search("Email_Address", email, useKeywordAnalyzer: true);
    }

    private List<Customer_Information> Search(string field, string searchTerm, bool useKeywordAnalyzer = false)
    {
        FSDirectory directory = FSDirectory.Open(new DirectoryInfo(IndexPath));
        IndexReader reader = DirectoryReader.Open(directory);
        IndexSearcher searcher = new IndexSearcher(reader);

        Analyzer analyzer = useKeywordAnalyzer
            ? new KeywordAnalyzer()
            : new StandardAnalyzer(Lucene.Net.Util.LuceneVersion.LUCENE_48);

        QueryParser parser = new QueryParser(Lucene.Net.Util.LuceneVersion.LUCENE_48, field, analyzer);
        Query query = parser.Parse(searchTerm);

        TopDocs docs = searcher.Search(query, 10);

        List<Customer_Information> results = new List<Customer_Information>();

        foreach (ScoreDoc scoreDoc in docs.ScoreDocs)
        {
            Document doc = searcher.Doc(scoreDoc.Doc);
            results.Add(new Customer_Information
            {
                Id = int.Parse(doc.Get("Id")),
                Name = doc.Get("Name"),
                Address = doc.Get("Address"),
                Email_Address = doc.Get("Email_Address")
            });
        }

        reader.Dispose();
        directory.Dispose();

        return results;
    }

    private void IndexCustomer(IndexWriter writer, Customer_Information customer)
    {
        Document doc = new Document();
        doc.Add(new StringField("Id", customer.Id.ToString(), Field.Store.YES));
        doc.Add(new TextField("Name", customer.Name, Field.Store.YES));
        doc.Add(new TextField("Address", customer.Address, Field.Store.YES));
        doc.Add(new StringField("Email_Address", customer.Email_Address, Field.Store.YES));

        writer.AddDocument(doc);
    }
}