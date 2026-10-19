
<?php

use Illuminate\Database\Migrations\Migration;
use Illuminate\Database\Schema\Blueprint;
use Illuminate\Support\Facades\Schema;

return new class extends Migration
{
    public function up(): void
    {
        Schema::create('elder_users', function (Blueprint $table) {
            $table->id();
            $table->string('name', 100);
            $table->string('firebase_uid', 128)->unique();
            $table->dateTime('created_at')->useCurrent();
            $table->dateTime('updated_at')
                ->useCurrent()
                ->useCurrentOnUpdate();
        });
    }

    public function down(): void
    {
        Schema::dropIfExists('elder_users');
    }
};
