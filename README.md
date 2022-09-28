# Deploy Command
```
gcloud functions deploy image-annotator   --runtime dotnet3   --trigger-event google.storage.object.finalize   --trigger-resource [bucket-name]   --entry-point=StorageImageAnnotator.Function --allow-unauthenticatedu
```

# Reference  
### Sample  
- Base sample  
https://github.com/GoogleCloudPlatform/functions-framework-dotnet/blob/main/examples/Google.Cloud.Functions.Examples.StorageImageAnnotator/Function.cs  

- TS sample  
https://qiita.com/imamurh/items/b8586a4c6a4b0084b555  

### gcloud Command  
- Quick Start  
https://cloud.google.com/functions/docs/create-deploy-gcloud?hl=ja#functions-prepare-environment-csharp  

- Basic   
https://cloud.google.com/functions/docs/deploy?hl=ja  
- Cloud Storage Trigger  
https://cloud.google.com/functions/docs/calling/storage?hl=ja  

### Runtime  
https://cloud.google.com/functions/docs/concepts/execution-environment?hl=ja#runtimes  

### Test  
https://cloud.google.com/functions/docs/testing/test-event?hl=ja#storage-triggered_functions  

### Meta Data  
- Summary  
https://cloud.google.com/storage/docs/metadata?hl=ja#content-encoding  
- variable Content-Type  
https://www.iana.org/assignments/media-types/media-types.xhtml  

